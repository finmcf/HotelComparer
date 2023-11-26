using Newtonsoft.Json;
using System.Collections.Generic;

namespace HotelComparer.Models
{
    public class AutosuggestResponse
    {
        [JsonProperty("items")]
        public List<AutosuggestItem> Items { get; set; }
    }

    public class AutosuggestItem
    {
        public string Title { get; set; }
        public string Id { get; set; }
        public string ResultType { get; set; }
        public string Href { get; set; }
        public Highlights Highlights { get; set; }
        public Address Address { get; set; }
        public Position Position { get; set; }
        public List<Access> Access { get; set; }
        public int Distance { get; set; }
        public List<Category> Categories { get; set; }
        public List<Chain> Chains { get; set; }
        public List<Reference> References { get; set; }
    }

    public class Highlights
    {
        public List<HighlightRange> Title { get; set; }
        public List<HighlightRange> Address { get; set; }
    }

    public class HighlightRange
    {
        public int Start { get; set; }
        public int End { get; set; }
    }

    public class Address
    {
        public string Label { get; set; }
    }

    public class Position
    {
        public double Lat { get; set; }
        public double Lng { get; set; }
    }

    public class Access
    {
        public double Lat { get; set; }
        public double Lng { get; set; }
    }

    public class Category
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public bool Primary { get; set; }
    }

    public class Chain
    {
        public string Id { get; set; }
        public string Name { get; set; }
    }

    public class Reference
    {
        public Supplier Supplier { get; set; }
        public string Id { get; set; }
    }

    public class Supplier
    {
        public string Id { get; set; }
    }
}
