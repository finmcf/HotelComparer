using HotelComparer.Models; // Make sure this using directive is present
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HotelComparer.Services
{
    public interface IAmadeusAutocompleteService
    {
        Task<IEnumerable<HotelAutocompleteResult>> GetHotelAutocompleteSuggestions(string keyword);
    }
}
