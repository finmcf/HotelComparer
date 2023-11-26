using HotelComparer.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HotelComparer.Services
{
    public interface IAmadeusAutocompleteService
    {
        Task<IEnumerable<HotelSuggestion>> GetHotelAutocompleteSuggestions(string keyword);
    }
}
