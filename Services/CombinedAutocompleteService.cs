using HotelComparer.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HotelComparer.Services
{
    public class CombinedAutocompleteService : ICombinedAutocompleteService
    {
        private readonly IHereAutosuggestService _hereAutosuggestService;
        private readonly IAmadeusAutocompleteService _amadeusAutocompleteService;

        public CombinedAutocompleteService(IHereAutosuggestService hereAutosuggestService, IAmadeusAutocompleteService amadeusAutocompleteService)
        {
            _hereAutosuggestService = hereAutosuggestService;
            _amadeusAutocompleteService = amadeusAutocompleteService;
        }

        public async Task<IEnumerable<CombinedAutocompleteResult>> GetCombinedAutocompleteSuggestions(string keyword, double latitude, double longitude)
        {
            var hereResults = await _hereAutosuggestService.GetLocationSuggestions(keyword, latitude, longitude);
            var amadeusResults = await _amadeusAutocompleteService.GetHotelAutocompleteSuggestions(keyword);

            var combinedResults = new List<CombinedAutocompleteResult>();

            // Add Here API results
            combinedResults.AddRange(hereResults.Select(item => new CombinedAutocompleteResult
            {
                Title = item.Title,
                Latitude = item.Position?.Lat,
                Longitude = item.Position?.Lng,
                Source = "HERE API"
            }));

            // Add Amadeus API results
            combinedResults.AddRange(amadeusResults.Select(hotel => new CombinedAutocompleteResult
            {
                Title = hotel.Name,
                Latitude = hotel.GeoCode?.Latitude,
                Longitude = hotel.GeoCode?.Longitude,
                Source = "Amadeus API"
            }));

            return combinedResults;
        }
    }

    public interface ICombinedAutocompleteService
    {
        Task<IEnumerable<CombinedAutocompleteResult>> GetCombinedAutocompleteSuggestions(string keyword, double latitude, double longitude);
    }

    public class CombinedAutocompleteResult
    {
        public string Title { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public string Source { get; set; }
    }
}
