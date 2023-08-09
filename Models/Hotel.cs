using Newtonsoft.Json;

namespace HotelComparer.Models
{
    public class Hotel
    {
        [JsonProperty("type")]
        public string Type { get; set; } = string.Empty;

        [JsonProperty("hotelId")]
        public string HotelId { get; set; } = string.Empty;

        [JsonProperty("chainCode")]
        public string ChainCode { get; set; } = string.Empty;

        [JsonProperty("dupeId")]
        public string DupeId { get; set; } = string.Empty;

        [JsonProperty("name")]
        public string Name { get; set; } = string.Empty;

        [JsonProperty("cityCode")]
        public string CityCode { get; set; } = string.Empty;

        [JsonProperty("latitude")]
        public double Latitude { get; set; }

        [JsonProperty("longitude")]
        public double Longitude { get; set; }
    }
}
