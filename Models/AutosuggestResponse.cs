namespace HotelComparer.Models
{
    using System;
    using System.Collections.Generic;

    using System.Globalization;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    public partial class AutosuggestResponse
    {
        [JsonProperty("items")]
        public AutosuggestItem[] Items { get; set; }

        [JsonProperty("queryTerms")]
        public object[] QueryTerms { get; set; }
    }

    public partial class AutosuggestItem
    {
        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("resultType")]
        public string ResultType { get; set; }

        [JsonProperty("localityType", NullValueHandling = NullValueHandling.Ignore)]
        public string LocalityType { get; set; }

        [JsonProperty("address", NullValueHandling = NullValueHandling.Ignore)]
        public ItemAddress Address { get; set; }

        [JsonProperty("position", NullValueHandling = NullValueHandling.Ignore)]
        public Position Position { get; set; }

        [JsonProperty("distance", NullValueHandling = NullValueHandling.Ignore)]
        public long? Distance { get; set; }

        [JsonProperty("mapView", NullValueHandling = NullValueHandling.Ignore)]
        public MapView MapView { get; set; }

        [JsonProperty("highlights")]
        public Highlights Highlights { get; set; }

        [JsonProperty("href", NullValueHandling = NullValueHandling.Ignore)]
        public Uri Href { get; set; }

        [JsonProperty("administrativeAreaType", NullValueHandling = NullValueHandling.Ignore)]
        public string AdministrativeAreaType { get; set; }
    }

    public partial class ItemAddress
    {
        [JsonProperty("label")]
        public string Label { get; set; }
    }

    public partial class Highlights
    {
        [JsonProperty("title")]
        public Title[] Title { get; set; }

        [JsonProperty("address", NullValueHandling = NullValueHandling.Ignore)]
        public HighlightsAddress Address { get; set; }
    }

    public partial class HighlightsAddress
    {
        [JsonProperty("label")]
        public Title[] Label { get; set; }
    }

    public partial class Title
    {
        [JsonProperty("start")]
        public long Start { get; set; }

        [JsonProperty("end")]
        public long End { get; set; }
    }

    public partial class MapView
    {
        [JsonProperty("west")]
        public double West { get; set; }

        [JsonProperty("south")]
        public double South { get; set; }

        [JsonProperty("east")]
        public double East { get; set; }

        [JsonProperty("north")]
        public double North { get; set; }
    }

    public partial class Position
    {
        [JsonProperty("lat")]
        public double Lat { get; set; }

        [JsonProperty("lng")]
        public double Lng { get; set; }
    }
}
