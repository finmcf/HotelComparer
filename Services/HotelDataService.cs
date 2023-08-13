using System.Collections.Generic;
using System.Threading.Tasks;
using HotelComparer.Models;

namespace HotelComparer.Services
{
    public class HotelDataService : IHotelDataService
    {
        private readonly IAmadeusApiService _amadeusApiService;

        public HotelDataService(IAmadeusApiService amadeusApiService)
        {
            _amadeusApiService = amadeusApiService;
        }

        public async Task<IEnumerable<string>> GetRawHotelDataAsync(HotelSearchRequest request)
        {
            return await _amadeusApiService.GetAmadeusResponses(request);
        }
    }
}
