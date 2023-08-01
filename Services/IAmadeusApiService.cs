using HotelComparer.Models;

using System.Collections.Generic;

namespace HotelComparer.Services
{
    public interface IAmadeusApiService
    {
        IEnumerable<string> GenerateUrls(HotelSearchRequest request);
    }
}
