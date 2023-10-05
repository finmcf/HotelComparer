using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations; // Import this namespace

namespace HotelComparer.Models
{
    public class HotelSearchRequest
    {
        [Required] // This attribute makes the parameter required
        public List<string> HotelIds { get; set; }

        [Required] // This attribute makes the parameter required
        public DateTime? CheckInDate { get; set; }

        [Required] // This attribute makes the parameter required
        public DateTime? CheckOutDate { get; set; }

        // Optional parameters with default values
        public int Adults { get; set; } = 1;
        public string CountryOfResidence { get; set; } = "US";
        public int RoomQuantity { get; set; } = 1;
        public string PriceRange { get; set; } = "100-5000";
        public string Currency { get; set; } = "USD";
        public string PaymentPolicy { get; set; } = "NONE";
        public string BoardType { get; set; } = "ROOM_ONLY";
        public bool IncludeClosed { get; set; } = true;
        public bool BestRateOnly { get; set; } = true;
        public string Language { get; set; } = "ENG";  // Added Language parameter

        public HotelSearchRequest()
        {
            HotelIds = new List<string>();
        }
    }
}
