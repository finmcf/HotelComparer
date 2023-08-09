using System.Collections.Generic;

namespace HotelComparer.Models
{
    public class Root
    {
        public List<Data> Data { get; set; } = new List<Data>();
        public Dictionaries Dictionaries { get; set; } = new Dictionaries();
    }
}
