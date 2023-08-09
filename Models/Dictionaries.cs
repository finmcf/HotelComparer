using System.Collections.Generic;

namespace HotelComparer.Models
{
    public class Dictionaries
    {
        public Dictionary<string, CurrencyConversionLookupRates> CurrencyConversionLookupRates { get; set; } = new Dictionary<string, CurrencyConversionLookupRates>();
    }

    public class CurrencyConversionLookupRates
    {
        public string Rate { get; set; } = string.Empty;
        public string Target { get; set; } = string.Empty;
        public int TargetDecimalPlaces { get; set; }
    }
}