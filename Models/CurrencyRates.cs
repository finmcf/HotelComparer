using System.Collections.Generic;

namespace HotelComparer.Models
{
    public class CurrencyRates
    {
        public bool Valid { get; set; }
        public long Updated { get; set; }
        public string Base { get; set; }
        public Dictionary<string, decimal> Rates { get; set; }
    }
}
