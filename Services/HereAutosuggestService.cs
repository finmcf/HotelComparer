﻿using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using HotelComparer.Models;
using System.Linq;
using Microsoft.Extensions.Logging; // Importing the ILogger namespace

namespace HotelComparer.Services
{
    public class HereAutosuggestService : IHereAutosuggestService
    {
        private const string AutosuggestApiUrl = "https://autosuggest.search.hereapi.com/v1/autosuggest";
        private readonly IHereApiTokenService _hereApiTokenService;
        private readonly HttpClient _httpClient;
        private readonly ILogger<HereAutosuggestService> _logger; // Logger dependency

        private readonly double DefaultLatitude = 51.5074;
        private readonly double DefaultLongitude = -0.1278;

        public HereAutosuggestService(IHereApiTokenService hereApiTokenService, HttpClient httpClient, ILogger<HereAutosuggestService> logger) // Include logger in constructor
        {
            _hereApiTokenService = hereApiTokenService ?? throw new ArgumentNullException(nameof(hereApiTokenService));
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _logger = logger; // Initialize the logger
        }

        public async Task<IEnumerable<HotelSuggestion>> GetLocationSuggestions(string keyword, double latitude = 0, double longitude = 0)
        {
            if (string.IsNullOrWhiteSpace(keyword))
            {
                throw new ArgumentException("Keyword must not be null or whitespace.", nameof(keyword));
            }

            if (latitude == 0 && longitude == 0)
            {
                latitude = DefaultLatitude;
                longitude = DefaultLongitude;
            }

            try
            {
                _logger.LogInformation("Retrieving access token for HERE API.");
                string accessToken = await _hereApiTokenService.GetAccessTokenAsync();
                if (string.IsNullOrEmpty(accessToken))
                {
                    _logger.LogError("Failed to retrieve access token for HERE API.");
                    return new List<HotelSuggestion>();
                }
                _logger.LogInformation("Access token retrieved successfully.");

                var requestUrl = $"{AutosuggestApiUrl}?q={Uri.EscapeDataString(keyword)}&at={latitude},{longitude}&lang=en";
                _logger.LogInformation($"Sending request to HERE API: {requestUrl}");

                _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);
                HttpResponseMessage response = await _httpClient.GetAsync(requestUrl);

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError($"HERE API request failed with status code: {response.StatusCode}");
                    return new List<HotelSuggestion>();
                }

                string jsonResponse = await response.Content.ReadAsStringAsync();
                _logger.LogInformation($"Received response from HERE API.");

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
                        HotelIds = new List<string>()
                    })
                    .ToList() ?? new List<HotelSuggestion>();

                return suggestions;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error fetching data from HERE API: {ex.Message}");
                return new List<HotelSuggestion>();
            }
        }

        private bool IsHotel(AutosuggestItem item)
        {
            return item.ResultType == "chainQuery" ||
                   item.Categories?.Any(cat => cat.Name.ToLower().Contains("hotel")) == true;
        }
    }

    public interface IHereAutosuggestService
    {
        Task<IEnumerable<HotelSuggestion>> GetLocationSuggestions(string keyword, double latitude = 0, double longitude = 0);
    }
}
