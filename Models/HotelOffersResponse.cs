using System;
using System.Collections.Generic;

namespace HotelPriceComparer.Models
{
    public class HotelOffersResponse
    {
        public List<HotelOffer> Data { get; set; }
    }

    public class HotelOffer
    {
        public string Type { get; set; }
        public HotelData Hotel { get; set; }
        public bool Available { get; set; }
        public List<OfferData> Offers { get; set; }
    }

    public class HotelData
    {
        public string HotelId { get; set; }
        public string Name { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }

    public class OfferData
    {
        public string Id { get; set; }
        public DateTime CheckInDate { get; set; }
        public DateTime CheckOutDate { get; set; }
        public RoomData Room { get; set; }
        public GuestData Guests { get; set; }
        public PriceData Price { get; set; }
        public PoliciesData Policies { get; set; }
    }

    public class RoomData
    {
        public string Type { get; set; }
        public RoomTypeEstimatedData TypeEstimated { get; set; }
        public RoomDescriptionData Description { get; set; }
    }

    public class RoomTypeEstimatedData
    {
        public string Category { get; set; }
        public int Beds { get; set; }
        public string BedType { get; set; }
    }

    public class RoomDescriptionData
    {
        public string Text { get; set; }
    }

    public class GuestData
    {
        public int Adults { get; set; }
    }

    public class PriceData
    {
        public string Currency { get; set; }
        public string Total { get; set; }
    }

    public class PoliciesData
    {
        public string PaymentType { get; set; }
    }
}
