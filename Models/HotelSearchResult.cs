using System.Collections.Generic;

namespace HotelPriceComparer.Models
{
    public class HotelSearchResult
    {
        public string HotelId { get; set; }
        public string HotelName { get; set; }
        public bool IsAvailable { get; set; }
        public List<HotelOffer> Offers { get; set; }

        public HotelSearchResult()
        {
            Offers = new List<HotelOffer>(); // Initialize with an empty list
        }
    }
}
