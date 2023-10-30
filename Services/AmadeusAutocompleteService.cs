using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

using Newtonsoft.Json;

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

        public async Task<IEnumerable<HotelAutocompleteResult>> GetHotelAutocompleteSuggestions(string keyword)
        {
            if (string.IsNullOrWhiteSpace(keyword))
            {
                throw new ArgumentException("Keyword must not be null or whitespace.", nameof(keyword));
            }

            string accessToken = await _amadeusApiTokenService.GetAccessTokenAsync();
            if (string.IsNullOrEmpty(accessToken))
            {
                throw new InvalidOperationException("Failed to obtain an access token.");
            }

            var requestUrl = $"{AutocompleteApiUrl}?keyword={Uri.EscapeDataString(keyword)}&subType=HOTEL_LEISURE"; // Customize query parameters as needed

            _httpClient.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);

            HttpResponseMessage response = await _httpClient.GetAsync(requestUrl);
            response.EnsureSuccessStatusCode();

            string jsonResponse = await response.Content.ReadAsStringAsync();
            var autocompleteData = JsonConvert.DeserializeObject<HotelAutocompleteResponse>(jsonResponse);

            return autocompleteData?.Data ?? new List<HotelAutocompleteResult>();
        }
    }

    // Models to match the JSON structure returned by Amadeus API.
    public class HotelAutocompleteResponse
    {
        [JsonProperty("data")]
        public List<HotelAutocompleteResult> Data { get; set; }
    }

    public class HotelAutocompleteResult
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        // Add other properties as needed based on the Amadeus API response
    }
}
