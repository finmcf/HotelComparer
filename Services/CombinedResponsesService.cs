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
        private readonly ILogger<CombinedResponsesService> _logger; // Logger dependency

        public CombinedResponsesService(
            IAmadeusAutocompleteService amadeusService,
            IHereAutosuggestService hereService,
            ILogger<CombinedResponsesService> logger) // Inject ILogger
        {
            _amadeusService = amadeusService;
            _hereService = hereService;
            _logger = logger; // Initialize the logger
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

            var combinedSuggestions = amadeusSuggestions.Concat(hereSuggestions);
            return combinedSuggestions;
        }
    }

    public interface ICombinedResponsesService
    {
        Task<IEnumerable<HotelSuggestion>> GetCombinedSuggestions(string keyword, double? latitude = null, double? longitude = null);
    }
}
