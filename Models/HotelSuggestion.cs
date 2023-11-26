using System.Collections.Generic;

namespace HotelComparer.Models
{
    public class CombinedResponse
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string Type { get; set; }
        public string Source { get; set; } = "HERE";
        public List<string> HotelIds { get; set; }
        public string Address { get; set; }
    }
}