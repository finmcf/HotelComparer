using System;

namespace HotelPriceComparer.Models
{
    public class HotelSearchRequest
    {
        public string Location { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public HotelSearchRequest()
        {
            Location = string.Empty; // Initialize with empty string
        }
    }
}
