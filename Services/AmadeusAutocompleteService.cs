using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using HotelComparer.Models;
using System.Linq;

namespace HotelComparer.Services
{
    public class AmadeusAutocompleteService : IAmadeusAutocompleteService
    {
        private const string AutocompleteApiUrl = "https://test.api.amadeus.com/v1/reference-data/locations/hotel";
        private readonly IAmadeusApiTokenService _amadeusApiTokenService;
        private readonly HttpClient _httpClient;

        public AmadeusAutocompleteService(IAmadeusApiTokenService amadeusApiTokenService, HttpClient httpClient)
        {
            _amadeusApiTokenService = amadeusApiTokenService ?? throw new ArgumentNullException(nameof(amadeusApiTokenService));
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        }

        public async Task<IEnumerable<HotelSuggestion>> GetHotelAutocompleteSuggestions(string keyword)
        {
            if (string.IsNullOrWhiteSpace(keyword))
            {
                throw new ArgumentException("Keyword must not be null or whitespace.", nameof(keyword));
            }

            string accessToken = _amadeusApiTokenService.GetCachedAccessToken();
            if (string.IsNullOrEmpty(accessToken))
            {
                accessToken = await _amadeusApiTokenService.GetAccessTokenAsync();
            }

            if (string.IsNullOrEmpty(accessToken))
            {
                throw new InvalidOperationException("Failed to obtain an access token.");
            }

            var requestUrl = $"{AutocompleteApiUrl}?keyword={Uri.EscapeDataString(keyword)}&subType=HOTEL_LEISURE";

            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);

            HttpResponseMessage response = await _httpClient.GetAsync(requestUrl);
            response.EnsureSuccessStatusCode();

            string jsonResponse = await response.Content.ReadAsStringAsync();
            var autocompleteData = JsonConvert.DeserializeObject<HotelAutocompleteResponse>(jsonResponse);

            var suggestions = autocompleteData?.Data
                .Select(item => new HotelSuggestion
                {
                    Id = item.Id.ToString(),
                    Name = item.Name,
                    Latitude = item.GeoCode.Latitude,
                    Longitude = item.GeoCode.Longitude,
                    Type = "Hotel",
                    Source = "Amadeus",
                    HotelIds = item.HotelIds ?? new List<string>(), // Ensure this property is correctly populated
                    Address = $"{item.Address.CityName}, {item.Address.CountryCode}"
                })
                .ToList() ?? new List<HotelSuggestion>();

            return suggestions;
        }
    }

    public class HotelAutocompleteResponse
    {
        [JsonProperty("data")]
        public List<HotelAutocompleteResult> Data { get; set; }
    }

    public class HotelAutocompleteResult
    {
        public int Id { get; set; }
        public string Name { get; set; }
        // Other properties as per the Amadeus API response structure
        public List<string> HotelIds { get; set; }
        public Address Address { get; set; }
        public GeoCode GeoCode { get; set; }
    }

    public class Address
    {
        public string CityName { get; set; }
        public string CountryCode { get; set; }
    }

    public class GeoCode
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }
}

