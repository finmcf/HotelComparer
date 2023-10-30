namespace HotelComparer.Models
{
    using System.Collections.Generic;
    using Newtonsoft.Json;

    public class HotelAutocompleteResult
    {
        [JsonProperty("id")]
        public int Id { get; set; } // Assuming ID will always be provided

        [JsonProperty("name")]
        public string Name { get; set; } = string.Empty;

        [JsonProperty("iataCode")]
        public string IataCode { get; set; } = string.Empty;

        [JsonProperty("subType")]
        public string SubType { get; set; } = string.Empty;

        [JsonProperty("relevance")]
        public int Relevance { get; set; } // Assuming relevance will always be provided

        [JsonProperty("type")]
        public string Type { get; set; } = string.Empty;

        [JsonProperty("hotelIds")]
        public List<string> HotelIds { get; set; } = new List<string>();

        [JsonProperty("address")]
        public HotelAddress Address { get; set; } = new HotelAddress();

        [JsonProperty("geoCode")]
        public GeoCode GeoCode { get; set; } = new GeoCode();
    }

    public class HotelAddress
    {
        [JsonProperty("cityName")]
        public string CityName { get; set; } = string.Empty;

        [JsonProperty("countryCode")]
        public string CountryCode { get; set; } = string.Empty;
    }

    public class GeoCode
    {
        [JsonProperty("latitude")]
        public double Latitude { get; set; }

        [JsonProperty("longitude")]
        public double Longitude { get; set; }
    }
}
