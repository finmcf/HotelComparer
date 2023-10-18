using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using HotelComparer.Models;

namespace HotelComparer.Services
{
    public class AmadeusApiService : IAmadeusApiService
    {
        private const string AMADEUS_API_URL = "https://test.api.amadeus.com/v3/shopping/hotel-offers";
        private readonly IAmadeusApiTokenService _amadeusApiTokenService;
        private static SemaphoreSlim _semaphore = new SemaphoreSlim(10); // Control concurrency

        public AmadeusApiService(IAmadeusApiTokenService amadeusApiTokenService)
        {
            _amadeusApiTokenService = amadeusApiTokenService;
        }

        public async Task<IEnumerable<string>> GetAmadeusResponses(HotelSearchRequest request)
        {
            var urls = GenerateUrls(request);
            var tasks = new List<Task<string>>();

            foreach (var url in urls)
            {
                tasks.Add(SendRequestWithThrottlingAndRetry(url));
            }

            // Awaiting all tasks to complete
            var responses = await Task.WhenAll(tasks);
            return responses;
        }

        private IEnumerable<string> GenerateUrls(HotelSearchRequest request)
        {
            ValidateRequestDates(request);

            var dateRanges = GenerateDateRanges(request.CheckInDate.Value, request.CheckOutDate.Value);
            var urls = new List<string>();
            string hotels = string.Join(",", request.HotelIds);

            foreach (var dateRange in dateRanges)
            {
                var url = $"{AMADEUS_API_URL}?hotelIds={hotels}&adults={request.Adults}&checkInDate={dateRange.Item1.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)}&checkOutDate={dateRange.Item2.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)}&countryOfResidence={request.CountryOfResidence}&roomQuantity={request.RoomQuantity}&priceRange={request.PriceRange}&currency={request.Currency}&paymentPolicy={request.PaymentPolicy}&boardType={request.BoardType}&includeClosed={request.IncludeClosed.ToString().ToLower()}&bestRateOnly={request.BestRateOnly.ToString().ToLower()}&lang={request.Language}";
                urls.Add(url);
            }

            return urls;
        }

        private void ValidateRequestDates(HotelSearchRequest request)
        {
            if (!request.CheckInDate.HasValue)
            {
                throw new ArgumentNullException(nameof(request.CheckInDate), "Check-in date cannot be null.");
            }

            if (!request.CheckOutDate.HasValue)
            {
                throw new ArgumentNullException(nameof(request.CheckOutDate), "Check-out date cannot be null.");
            }

            if (request.CheckInDate.Value.Date < DateTime.UtcNow.Date)
            {
                throw new ArgumentException("Check-in date cannot be in the past.");
            }

            if (request.CheckInDate.Value >= request.CheckOutDate.Value)
            {
                throw new ArgumentException("Check-in date must be before Check-out date.");
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

        private async Task<string> SendRequestWithThrottlingAndRetry(string url)
        {
            await _semaphore.WaitAsync();  // Wait for the semaphore

            try
            {
                await Task.Delay(100);  // Ensure a gap between requests to respect rate limit
                return await SendRequestToAmadeusAsync(url);
            }
            finally
            {
                _semaphore.Release();  // Release the semaphore after completion
            }
        }

        private async Task<string> SendRequestToAmadeusAsync(string url)
        {
            using (HttpClient client = new HttpClient())
            {
                string accessToken = await _amadeusApiTokenService.GetAccessTokenAsync();
                if (string.IsNullOrEmpty(accessToken))
                {
                    throw new ArgumentException("Failed to obtain an access token.");
                }

                client.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);

                try
                {
                    HttpResponseMessage response = await client.GetAsync(url);
                    response.EnsureSuccessStatusCode();
                    return await response.Content.ReadAsStringAsync();
                }
                catch (HttpRequestException e)
                {
                    throw new Exception($"Request error for URL {url}: {e.Message}");
                }
            }
        }
    }
}
