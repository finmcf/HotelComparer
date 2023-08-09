using System.Collections.Generic;

namespace HotelComparer.Models
{
    public class Data
    {
        public string Type { get; set; } = string.Empty;
        public Hotel Hotel { get; set; } = new Hotel();
        public bool Available { get; set; }
        public List<Offer> Offers { get; set; } = new List<Offer>();
        public string Self { get; set; } = string.Empty;
    }
}
