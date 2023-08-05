using HotelComparer.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HotelComparer.Services
{
    public interface IAmadeusApiService
    {
        Task<IEnumerable<string>> GenerateUrls(HotelSearchRequest request);
    }
}
