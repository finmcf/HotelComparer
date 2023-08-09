using Newtonsoft.Json;
using System.Collections.Generic;

namespace HotelComparer.Models
{
    public class Offer
    {
        [JsonProperty("id")]
        public string Id { get; set; } = string.Empty;

        [JsonProperty("checkInDate")]
        public string CheckInDate { get; set; } = string.Empty;

        [JsonProperty("checkOutDate")]
        public string CheckOutDate { get; set; } = string.Empty;

        [JsonProperty("rateCode")]
        public string RateCode { get; set; } = string.Empty;

        [JsonProperty("rateFamilyEstimated")]
        public RateFamilyEstimated RateFamilyEstimated { get; set; } = new RateFamilyEstimated();

        [JsonProperty("room")]
        public Room Room { get; set; } = new Room();

        [JsonProperty("guests")]
        public Guests Guests { get; set; } = new Guests();

        [JsonProperty("price")]
        public Price Price { get; set; } = new Price();

        [JsonProperty("policies")]
        public Policies Policies { get; set; } = new Policies();

        [JsonProperty("self")]
        public string Self { get; set; } = string.Empty;
    }

    public class RateFamilyEstimated
    {
        [JsonProperty("code")]
        public string Code { get; set; } = string.Empty;

        [JsonProperty("type")]
        public string Type { get; set; } = string.Empty;
    }

    public class Room
    {
        [JsonProperty("type")]
        public string Type { get; set; } = string.Empty;

        [JsonProperty("typeEstimated")]
        public TypeEstimated TypeEstimated { get; set; } = new TypeEstimated();

        [JsonProperty("description")]
        public Description Description { get; set; } = new Description();
    }

    public class TypeEstimated
    {
        [JsonProperty("category")]
        public string Category { get; set; } = string.Empty;

        [JsonProperty("beds")]
        public int Beds { get; set; }

        [JsonProperty("bedType")]
        public string BedType { get; set; } = string.Empty;
    }

    public class Description
    {
        [JsonProperty("text")]
        public string Text { get; set; } = string.Empty;

        [JsonProperty("lang")]
        public string Lang { get; set; } = string.Empty;
    }

    public class Guests
    {
        [JsonProperty("adults")]
        public int Adults { get; set; }
    }

    public class Price
    {
        [JsonProperty("currency")]
        public string Currency { get; set; } = string.Empty;

        [JsonProperty("base")]
        public string Base { get; set; } = string.Empty;

        [JsonProperty("total")]
        public string Total { get; set; } = string.Empty;

        [JsonProperty("variations")]
        public Variations Variations { get; set; } = new Variations();
    }

    public class Variations
    {
        [JsonProperty("average")]
        public Average Average { get; set; } = new Average();

        [JsonProperty("changes")]
        public List<Change> Changes { get; set; } = new List<Change>();
    }

    public class Average
    {
        [JsonProperty("base")]
        public string Base { get; set; } = string.Empty;
    }

    public class Change
    {
        [JsonProperty("startDate")]
        public string StartDate { get; set; } = string.Empty;

        [JsonProperty("endDate")]
        public string EndDate { get; set; } = string.Empty;

        [JsonProperty("total")]
        public string Total { get; set; } = string.Empty;
    }

    public class Policies
    {
        [JsonProperty("cancellations")]
        public List<Cancellation> Cancellations { get; set; } = new List<Cancellation>();

        [JsonProperty("paymentType")]
        public string PaymentType { get; set; } = string.Empty;
    }

    public class Cancellation
    {
        [JsonProperty("amount")]
        public string Amount { get; set; } = string.Empty;

        [JsonProperty("deadline")]
        public string Deadline { get; set; } = string.Empty;
    }
}
