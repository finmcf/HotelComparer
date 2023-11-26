using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using HotelComparer.Models;
using System.Linq;

namespace HotelComparer.Services
{
    public class HereAutosuggestService : IHereAutosuggestService
    {
        private const string AutosuggestApiUrl = "https://autosuggest.search.hereapi.com/v1/autosuggest";
        private readonly IHereApiTokenService _hereApiTokenService;
        private readonly HttpClient _httpClient;

        public HereAutosuggestService(IHereApiTokenService hereApiTokenService, HttpClient httpClient)
        {
            _hereApiTokenService = hereApiTokenService ?? throw new ArgumentNullException(nameof(hereApiTokenService));
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        }

        public async Task<IEnumerable<HotelSuggestion>> GetLocationSuggestions(string keyword, double latitude, double longitude)
        {
            if (string.IsNullOrWhiteSpace(keyword))
            {
                throw new ArgumentException("Keyword must not be null or whitespace.", nameof(keyword));
            }

            string accessToken = await _hereApiTokenService.GetAccessTokenAsync();
            var requestUrl = $"{AutosuggestApiUrl}?q={Uri.EscapeDataString(keyword)}&at={latitude},{longitude}&lang=en";

            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);
            HttpResponseMessage response = await _httpClient.GetAsync(requestUrl);
            response.EnsureSuccessStatusCode();

            string jsonResponse = await response.Content.ReadAsStringAsync();
            var autosuggestData = JsonConvert.DeserializeObject<AutosuggestResponse>(jsonResponse);

            var suggestions = autosuggestData?.Items
                .Where(item => !IsHotel(item))
                .Select(item => new HotelSuggestion
                {
                    Id = item.Id,
                    Name = item.Title,
                    Latitude = item.Position?.Lat ?? 0,
                    Longitude = item.Position?.Lng ?? 0,
                    Type = item.ResultType == "locality" ? "Locality" : "Place",
                    Address = item.Address?.Label,
                    HotelIds = new List<string>() // Empty list as HERE API does not provide hotel IDs
                })
                .ToList() ?? new List<HotelSuggestion>();

            return suggestions;
        }

        private bool IsHotel(AutosuggestItem item)
        {
            return item.ResultType == "chainQuery" ||
                   item.Categories?.Any(cat => cat.Name.ToLower().Contains("hotel")) == true;
        }
    }

    public interface IHereAutosuggestService
    {
        Task<IEnumerable<HotelSuggestion>> GetLocationSuggestions(string keyword, double latitude, double longitude);
    }
}
