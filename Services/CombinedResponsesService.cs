using HotelComparer.Models;
using Microsoft.Extensions.Logging;
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
                if (latitude.HasValue && longitude.HasValue)
                {
                    _logger.LogInformation("Fetching location suggestions from Here API.");
                    hereSuggestions = (await _hereService.GetLocationSuggestions(keyword, latitude.Value, longitude.Value)).ToList();
                    _logger.LogInformation("Received location suggestions from Here API.");
                }
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching suggestions from Here API.");
            }

            var combinedSuggestions = amadeusSuggestions.Concat(hereSuggestions)
                .OrderByDescending(s => CalculateSimilarity(keyword, s.Name))
                .ThenBy(s => GetTypePriority(s.Type))
                .ToList();

            return combinedSuggestions;
        }

        private double CalculateSimilarity(string keyword, string name)
        {
            // Simple similarity calculation (can be replaced with a more advanced algorithm)
            // For now, just counting the number of characters in common
            return name.Intersect(keyword).Count();
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
    }

    public interface ICombinedResponsesService
    {
        Task<IEnumerable<HotelSuggestion>> GetCombinedSuggestions(string keyword, double? latitude = null, double? longitude = null);
    }
}
