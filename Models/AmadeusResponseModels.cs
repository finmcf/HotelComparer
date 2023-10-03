using System;
using System.Collections.Generic;

namespace HotelComparer.Models
{
    public class HotelOfferData
    {
        public HotelInfo Hotel { get; set; }
        public List<HotelOffer> Offers { get; set; } = new List<HotelOffer>();
        public double CheapestCombination { get; set; }
        public List<string> CheapestOfferIds { get; set; } = new List<string>();  // Added this property
        public string Self { get; set; }
    }

    public class HotelInfo
    {
        public string Type { get; set; }
        public string HotelId { get; set; }
        public string ChainCode { get; set; }
        public string DupeId { get; set; }
        public string Name { get; set; }
        public string CityCode { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }

    public class HotelOffer
    {
        public string Id { get; set; }
        public DateTime CheckInDate { get; set; }
        public DateTime CheckOutDate { get; set; }
        public string RateCode { get; set; }
        public RateFamilyEstimated RateFamilyEstimated { get; set; }
        public HotelRoom Room { get; set; }
        public GuestInfo Guests { get; set; }
        public HotelPrice Price { get; set; }
        public PolicyInfo Policies { get; set; }
        public string Self { get; set; }
    }

    public class RateFamilyEstimated
    {
        public string Code { get; set; }
        public string Type { get; set; }
    }

    public class HotelRoom
    {
        public string Type { get; set; }
        public TypeEstimated TypeEstimated { get; set; }
        public RoomDescription Description { get; set; }
    }

    public class TypeEstimated
    {
        public string Category { get; set; }
        public int Beds { get; set; }
        public string BedType { get; set; }
    }

    public class RoomDescription
    {
        public string Text { get; set; }
        public string Lang { get; set; }
    }

    public class GuestInfo
    {
        public int Adults { get; set; }
    }

    public class HotelPrice
    {
        public string Currency { get; set; }
        public string Base { get; set; }
        public string Total { get; set; }
        public PriceVariations Variations { get; set; }
    }

    public class PriceVariations
    {
        public AveragePrice Average { get; set; }
        public List<PriceChange> Changes { get; set; }
    }

    public class AveragePrice
    {
        public string Base { get; set; }
    }

    public class PriceChange
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Total { get; set; }
    }

    public class PolicyInfo
    {
        public List<CancellationPolicy> Cancellations { get; set; }
        public string PaymentType { get; set; }
    }

    public class CancellationPolicy
    {
        public string Amount { get; set; }
        public DateTime Deadline { get; set; }
    }

    public class AmadeusApiResponse
    {
        public List<HotelOfferData> Data { get; set; }
        public AmadeusDictionaries Dictionaries { get; set; }
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
