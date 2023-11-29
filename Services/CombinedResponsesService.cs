using HotelComparer.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HotelComparer.Services
{
    public class CombinedResponsesService : ICombinedResponsesService
    {
        private readonly IAmadeusAutocompleteService _amadeusService;
        private readonly IHereAutosuggestService _hereService;
        private readonly ILogger<CombinedResponsesService> _logger;

        public CombinedResponsesService(
            IAmadeusAutocompleteService amadeusService,
            IHereAutosuggestService hereService,
            ILogger<CombinedResponsesService> logger)
        {
            _amadeusService = amadeusService;
            _hereService = hereService;
            _logger = logger;
        }

        public async Task<IEnumerable<HotelSuggestion>> GetCombinedSuggestions(string keyword, double? latitude, double? longitude)
        {
            List<HotelSuggestion> amadeusSuggestions = new List<HotelSuggestion>();
            List<HotelSuggestion> hereSuggestions = new List<HotelSuggestion>();

            try
            {
                _logger.LogInformation("Fetching hotel suggestions from Amadeus API.");
                amadeusSuggestions = (await _amadeusService.GetHotelAutocompleteSuggestions(keyword)).ToList();
                _logger.LogInformation("Received hotel suggestions from Amadeus API.");
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching suggestions from Amadeus API.");
            }

            try
            {
                _logger.LogInformation("Fetching location suggestions from Here API.");
                hereSuggestions = (await _hereService.GetLocationSuggestions(keyword, latitude ?? 0, longitude ?? 0)).ToList();
                _logger.LogInformation("Received location suggestions from Here API.");
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching suggestions from Here API.");
            }

            var combinedSuggestions = amadeusSuggestions.Concat(hereSuggestions)
                .OrderByDescending(s => CalculateWeightedScore(keyword, s.Name, s.Type))
                .ToList();

            return combinedSuggestions;
        }

        private double CalculateSimilarity(string keyword, string name)
        {
            int distance = LevenshteinDistance(keyword.ToLower(), name.ToLower());
            int maxLength = Math.Max(keyword.Length, name.Length);
            return (maxLength - distance) / (double)maxLength;
        }

        private double CalculateWeightedScore(string keyword, string name, string type)
        {
            double similarityScore = CalculateSimilarity(keyword, name);
            int typePriority = GetTypePriority(type);

            return similarityScore * 0.9 + (1 - typePriority) * 0.1;
        }

        private int GetTypePriority(string type)
        {
            switch (type)
            {
                case "Locality": return 1;
                case "Place": return 2;
                case "Hotel": return 3;
                default: return 4;
            }
        }

        private int LevenshteinDistance(string a, string b)
        {
            int lengthA = a.Length;
            int lengthB = b.Length;
            var distances = new int[lengthA + 1, lengthB + 1];

            for (int i = 0; i <= lengthA; distances[i, 0] = i++) { }
            for (int j = 0; j <= lengthB; distances[0, j] = j++) { }

            for (int i = 1; i <= lengthA; i++)
            {
                for (int j = 1; j <= lengthB; j++)
                {
                    int cost = (b[j - 1] == a[i - 1]) ? 0 : 1;
                    distances[i, j] = Math.Min(
                        Math.Min(distances[i - 1, j] + 1, distances[i, j - 1] + 1),
                        distances[i - 1, j - 1] + cost);
                }
            }

            return distances[lengthA, lengthB];
        }
    }

    public interface ICombinedResponsesService
    {
        Task<IEnumerable<HotelSuggestion>> GetCombinedSuggestions(string keyword, double? latitude = null, double? longitude = null);
    }
}
