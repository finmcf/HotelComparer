using System;

namespace HotelPriceComparer.Models
{
    public class HotelBookingOption
    {
        public string HotelName { get; set; } = string.Empty;
        public DateTime CheckInDate { get; set; }
        public DateTime CheckOutDate { get; set; }
        public decimal Price { get; set; }
    }
}
