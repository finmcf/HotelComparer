using System;
using System.Collections.Generic;

namespace HotelPriceComparer.Models
{
    public class HotelSearchRequest
    {
        public List<string> HotelIds { get; set; }
        public DateTime? CheckInDate { get; set; }
        public DateTime? CheckOutDate { get; set; }
        public int? Adults { get; set; }
        public string? CountryOfResidence { get; set; }
        public int? RoomQuantity { get; set; }
        public string? PriceRange { get; set; }
        public string? Currency { get; set; }
        public string? PaymentPolicy { get; set; }
        public string? BoardType { get; set; }
        public bool? IncludeClosed { get; set; }
        public bool? BestRateOnly { get; set; }
        public string? Lang { get; set; }

        public HotelSearchRequest()
        {
            HotelIds = new List<string>(); // Initialize with an empty list
        }
    }
}
