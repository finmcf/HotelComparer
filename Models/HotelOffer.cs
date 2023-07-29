using System;
using System.Collections.Generic;

namespace HotelPriceComparer.Models
{
    public class HotelOffer
    {
        public string Id { get; set; }
        public DateTime CheckInDate { get; set; }
        public DateTime CheckOutDate { get; set; }
        public string RateCode { get; set; }
        public string RoomType { get; set; }
        public int Adults { get; set; }
        public Price Price { get; set; }
        public string PaymentType { get; set; }
        public string CancellationPolicy { get; set; }
        public string Self { get; set; }
    }

    public class Price
    {
        public string Currency { get; set; }
        public decimal Base { get; set; }
        public decimal Total { get; set; }
    }
}
