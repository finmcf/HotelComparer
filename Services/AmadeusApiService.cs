using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using HotelComparer.Models;
using Microsoft.Extensions.Logging;

namespace HotelComparer.Services
{
    public class AmadeusApiService : IAmadeusApiService
    {
        private const string AMADEUS_API_URL = "https://test.api.amadeus.com/v3/shopping/hotel-offers";
        private const string AMADEUS_HOTEL_LIST_API_URL = "https://test.api.amadeus.com/v1/reference-data/locations/hotels/by-geocode";
        private readonly IAmadeusApiTokenService _amadeusApiTokenService;
        private static SemaphoreSlim _semaphore = new SemaphoreSlim(10);
        private readonly ILogger<AmadeusApiService> _logger;

        public AmadeusApiService(IAmadeusApiTokenService amadeusApiTokenService, ILogger<AmadeusApiService> logger)
        {
            _amadeusApiTokenService = amadeusApiTokenService;
            _logger = logger;
        }

        public async Task<IEnumerable<string>> GetAmadeusResponses(HotelSearchRequest request)
        {
            _logger.LogInformation("Starting GetAmadeusResponses method.");
            string accessToken = _amadeusApiTokenService.GetCachedAccessToken();
            if (string.IsNullOrEmpty(accessToken))
            {
                _logger.LogInformation("Access token not cached, obtaining new access token.");
                accessToken = await _amadeusApiTokenService.GetAccessTokenAsync();
            }

            if (string.IsNullOrEmpty(accessToken))
            {
                _logger.LogError("Failed to obtain an access token.");
                throw new InvalidOperationException("Failed to obtain an access token.");
            }

            List<string> hotelIds = new List<string>();
            if (request.HotelIds.Any())
            {
                _logger.LogInformation($"Hotel IDs provided in request: {string.Join(", ", request.HotelIds)}");
                hotelIds.AddRange(request.HotelIds);
                if (request.HasLatLng())
                {
                    _logger.LogInformation($"Request has latitude and longitude: {request.Latitude}, {request.Longitude}");
                    var nearbyHotelIds = await GetNearbyHotelIds(request.Latitude, request.Longitude, request.Radius, accessToken);
                    hotelIds = hotelIds.Union(nearbyHotelIds).ToList();
                }
            }
            else if (request.HasLatLng())
            {
                _logger.LogInformation($"Request has only latitude and longitude: {request.Latitude}, {request.Longitude}");
                hotelIds = await GetNearbyHotelIds(request.Latitude, request.Longitude, request.Radius, accessToken);
            }

            TrimHotelList(ref hotelIds, request.MaxHotels);
            var urls = GenerateUrlsWithHotelIds(hotelIds, request);
            var tasks = new List<Task<string>>();

            foreach (var url in urls)
            {
                tasks.Add(SendRequestWithThrottlingAndRetry(url, accessToken));
            }

            var responses = await Task.WhenAll(tasks);
            foreach (var response in responses)
            {
                _logger.LogInformation($"Received response from Amadeus API for URL: {response}");
            }
            return responses;
        }

        private async Task<List<string>> GetNearbyHotelIds(double latitude, double longitude, int radius, string accessToken)
        {
            _logger.LogInformation($"Fetching nearby hotel IDs with Latitude: {latitude}, Longitude: {longitude}, Radius: {radius}");
            var hotelListUrl = $"{AMADEUS_HOTEL_LIST_API_URL}?latitude={latitude}&longitude={longitude}&radius={radius}&radiusUnit=KM";
            var response = await SendRequestToAmadeusAsync(hotelListUrl, accessToken);
            _logger.LogInformation($"Response from GetNearbyHotelIds: {response}");
            var hotelListResponse = JsonConvert.DeserializeObject<HotelListResponse>(response);
            var hotelIds = hotelListResponse.Data.Select(h => h.HotelId).ToList();
            return hotelIds;
        }

        private IEnumerable<string> GenerateUrlsWithHotelIds(List<string> hotelIds, HotelSearchRequest request)
        {
            var dateRanges = GenerateDateRanges(request.CheckInDate.Value, request.CheckOutDate.Value);
            var urls = new List<string>();

            foreach (var dateRange in dateRanges)
            {
                var url = $"{AMADEUS_API_URL}?hotelIds={string.Join(",", hotelIds)}&adults={request.Adults}&checkInDate={dateRange.Item1.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)}&checkOutDate={dateRange.Item2.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)}&countryOfResidence={request.CountryOfResidence}&roomQuantity={request.RoomQuantity}&priceRange={request.PriceRange}&currency={request.Currency}&paymentPolicy={request.PaymentPolicy}&boardType={request.BoardType}&includeClosed={request.IncludeClosed.ToString().ToLower()}&bestRateOnly={request.BestRateOnly.ToString().ToLower()}&lang={request.Language}";
                urls.Add(url);
            }

            return urls;
        }

        private void TrimHotelList(ref List<string> hotelIds, int maxHotels)
        {
            if (hotelIds.Count > maxHotels)
            {
                hotelIds = hotelIds.Take(maxHotels).ToList();
            }
        }

        private static List<Tuple<DateTime, DateTime>> GenerateDateRanges(DateTime checkInDate, DateTime checkOutDate)
        {
            var dateRanges = new List<Tuple<DateTime, DateTime>>();

            for (var date = checkInDate; date < checkOutDate; date = date.AddDays(1))
            {
                for (var nextDate = date.AddDays(1); nextDate <= checkOutDate; nextDate = nextDate.AddDays(1))
                {
                    dateRanges.Add(new Tuple<DateTime, DateTime>(date, nextDate));
                }
            }

            return dateRanges;
        }

        private async Task<string> SendRequestWithThrottlingAndRetry(string url, string accessToken)
        {
            await _semaphore.WaitAsync();

            try
            {
                await Task.Delay(200);
                return await SendRequestToAmadeusAsync(url, accessToken);
            }
            finally
            {
                _semaphore.Release();
            }
        }

        private async Task<string> SendRequestToAmadeusAsync(string url, string accessToken)
        {
            _logger.LogInformation($"Sending request to Amadeus API: {url}");
            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);

                try
                {
                    HttpResponseMessage response = await client.GetAsync(url);
                    response.EnsureSuccessStatusCode();
                    var content = await response.Content.ReadAsStringAsync();
                    _logger.LogInformation($"Received response from Amadeus API.");
                    return content;
                }
                catch (HttpRequestException e)
                {
                    _logger.LogError($"Request error for URL {url}: {e.Message}");
                    throw;
                }
            }
        }

        private class HotelListResponse
        {
            public List<HotelData> Data { get; set; }
        }

        private class HotelData
        {
            public string HotelId { get; set; }
        }
    }
}
