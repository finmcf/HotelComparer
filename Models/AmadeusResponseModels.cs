using System;
using System.Collections.Generic;

namespace HotelComparer.Models
{
    public class HotelOfferData
    {
        public HotelInfo Hotel { get; set; }
        public List<HotelOffer> Offers { get; set; } = new List<HotelOffer>();
    }

    public class HotelInfo
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string ChainCode { get; set; }
        public string CityCode { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }

    public class HotelOffer
    {
        public string Id { get; set; }
        public DateTime CheckInDate { get; set; }
        public DateTime CheckOutDate { get; set; }
        public double Price { get; set; }
        public string Currency { get; set; }
        public string RoomType { get; set; }
        public string BedType { get; set; }
        public string Description { get; set; }
    }

    public class AmadeusApiResponse
    {
        public List<AmadeusHotelData> Data { get; set; }
        public AmadeusDictionaries Dictionaries { get; set; }
    }

    public class AmadeusHotelData
    {
        public AmadeusHotelInfo Hotel { get; set; }
        public List<AmadeusOffer> Offers { get; set; }
    }

    public class AmadeusHotelInfo
    {
        public string HotelId { get; set; }
        public string Name { get; set; }
        public string ChainCode { get; set; }
        public string CityCode { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }

    public class AmadeusOffer
    {
        public string Id { get; set; }
        public string CheckInDate { get; set; }
        public string CheckOutDate { get; set; }
        public AmadeusRoom Room { get; set; }
        public AmadeusPrice Price { get; set; }
    }

    public class AmadeusRoom
    {
        public string Type { get; set; }
        public AmadeusTypeEstimated TypeEstimated { get; set; }
        public AmadeusDescription Description { get; set; }
    }

    public class AmadeusTypeEstimated
    {
        public string Category { get; set; }
        public int Beds { get; set; }
        public string BedType { get; set; }
    }

    public class AmadeusDescription
    {
        public string Text { get; set; }
    }

    public class AmadeusPrice
    {
        public string Currency { get; set; }
        public string Base { get; set; }
    }

    public class AmadeusDictionaries
    {
        public Dictionary<string, AmadeusCurrencyConversion> CurrencyConversionLookupRates { get; set; }
    }

    public class AmadeusCurrencyConversion
    {
        public string Rate { get; set; }
        public string Target { get; set; }
        public int TargetDecimalPlaces { get; set; }
    }
}