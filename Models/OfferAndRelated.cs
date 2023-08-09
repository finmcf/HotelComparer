using System.Collections.Generic;

namespace HotelComparer.Models
{
    public class Offer
    {
        public string Id { get; set; } = string.Empty;
        public string CheckInDate { get; set; } = string.Empty;
        public string CheckOutDate { get; set; } = string.Empty;
        public string RateCode { get; set; } = string.Empty;
        public RateFamilyEstimated RateFamilyEstimated { get; set; } = new RateFamilyEstimated();
        public Room Room { get; set; } = new Room();
        public Guests Guests { get; set; } = new Guests();
        public Price Price { get; set; } = new Price();
        public Policies Policies { get; set; } = new Policies();
        public string Self { get; set; } = string.Empty;
    }

    public class RateFamilyEstimated
    {
        public string Code { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
    }

    public class Room
    {
        public string Type { get; set; } = string.Empty;
        public TypeEstimated TypeEstimated { get; set; } = new TypeEstimated();
        public Description Description { get; set; } = new Description();
    }

    public class TypeEstimated
    {
        public string Category { get; set; } = string.Empty;
        public int Beds { get; set; }
        public string BedType { get; set; } = string.Empty;
    }

    public class Description
    {
        public string Text { get; set; } = string.Empty;
        public string Lang { get; set; } = string.Empty;
    }

    public class Guests
    {
        public int Adults { get; set; }
    }

    public class Price
    {
        public string Currency { get; set; } = string.Empty;
        public string Base { get; set; } = string.Empty;
        public string Total { get; set; } = string.Empty;
        public Variations Variations { get; set; } = new Variations();
    }

    public class Variations
    {
        public Average Average { get; set; } = new Average();
        public List<Change> Changes { get; set; } = new List<Change>();
    }

    public class Average
    {
        public string Base { get; set; } = string.Empty;
    }

    public class Change
    {
        public string StartDate { get; set; } = string.Empty;
        public string EndDate { get; set; } = string.Empty;
        public string Total { get; set; } = string.Empty;
    }

    public class Policies
    {
        public List<Cancellation> Cancellations { get; set; } = new List<Cancellation>();
        public string PaymentType { get; set; } = string.Empty;
    }

    public class Cancellation
    {
        public string Amount { get; set; } = string.Empty;
        public string Deadline { get; set; } = string.Empty;
    }
}
