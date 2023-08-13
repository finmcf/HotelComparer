using System.Collections.Generic;
using System.Threading.Tasks;
using HotelComparer.Models;

namespace HotelComparer.Services
{
    public interface IHotelDataService
    {
        Task<IEnumerable<string>> GetRawHotelDataAsync(HotelSearchRequest request);
    }
}
