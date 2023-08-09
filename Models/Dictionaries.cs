using Newtonsoft.Json;
using System.Collections.Generic;

namespace HotelComparer.Models
{
    public class Dictionaries
    {
        [JsonProperty("currencyConversionLookupRates")]
        public Dictionary<string, CurrencyConversionLookupRates> CurrencyConversionLookupRates { get; set; } = new Dictionary<string, CurrencyConversionLookupRates>();
    }

    public class CurrencyConversionLookupRates
    {
        [JsonProperty("rate")]
        public double Rate { get; set; }

        [JsonProperty("target")]
        public string Target { get; set; } = string.Empty;

        [JsonProperty("targetDecimalPlaces")]
        public int TargetDecimalPlaces { get; set; }
    }
}
