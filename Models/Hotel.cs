namespace HotelComparer.Models
{
    public class Hotel
    {
        public string Type { get; set; } = string.Empty;
        public string HotelId { get; set; } = string.Empty;
        public string ChainCode { get; set; } = string.Empty;
        public string DupeId { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string CityCode { get; set; } = string.Empty;
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }
}
