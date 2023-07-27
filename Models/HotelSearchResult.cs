using System.Collections.Generic;
using System.Linq;

namespace HotelPriceComparer.Models
{
    public class HotelSearchResult
    {
        public List<HotelBookingOption> Results { get; set; } = new List<HotelBookingOption>();

        public decimal TotalPrice
        {
            get { return Results.Sum(result => result.Price); }
        }
    }
}
