using Newtonsoft.Json;
using System.Collections.Generic;

namespace HotelComparer.Models
{
    public class Data
    {
        [JsonProperty("type")]
        public string Type { get; set; } = string.Empty;

        [JsonProperty("hotel")]
        public Hotel Hotel { get; set; } = new Hotel();

        [JsonProperty("available")]
        public bool Available { get; set; }

        [JsonProperty("offers")]
        public List<Offer> Offers { get; set; } = new List<Offer>();

        [JsonProperty("self")]
        public string Self { get; set; } = string.Empty;
    }
}
