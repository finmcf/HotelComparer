using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
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

        public async Task<IEnumerable<Root>> GetDeserializedHotelDataAsync(HotelSearchRequest request)
        {
            var rawResponses = await GetRawHotelDataAsync(request);
            
            return rawResponses.Select(json => JsonConvert.DeserializeObject<Root>(json)).ToList();
        }
    }
}
