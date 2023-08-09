using Newtonsoft.Json;
using System.Collections.Generic;

namespace HotelComparer.Models
{
    public class Root
    {
        [JsonProperty("data")]
        public List<Data> Data { get; set; } = new List<Data>();

        [JsonProperty("dictionaries")]
        public Dictionaries Dictionaries { get; set; } = new Dictionaries();
    }
}
