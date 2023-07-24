using System;
using System.Collections.Generic;

namespace HotelPriceComparer.Models
{
    public class HotelSearchResult
    {
        public List<HotelBookingOption> CheapestCombination { get; set; } = new List<HotelBookingOption>();
        public decimal TotalPrice { get; set; }
    }

    public class HotelBookingOption
    {
        public DateTime CheckIn { get; set; }
        public DateTime CheckOut { get; set; }
        public decimal Price { get; set; }
    }
}
