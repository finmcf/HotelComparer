using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace HotelComparer.Models
{
    public class HotelSearchRequest : IValidatableObject
    {
        // No longer marked as Required
        public List<string> HotelIds { get; set; }

        [Required]
        public DateTime? CheckInDate { get; set; }

        [Required]
        public DateTime? CheckOutDate { get; set; }

        // Optional parameters with default values
        public int Adults { get; set; } = 1;
        public string CountryOfResidence { get; set; } = "US";
        public int RoomQuantity { get; set; } = 1;
        public string PriceRange { get; set; } = "100-5000";
        public string Currency { get; set; } = "USD";
        public string PaymentPolicy { get; set; } = "NONE";
        public string BoardType { get; set; } = "ROOM_ONLY";
        public bool IncludeClosed { get; set; } = true;
        public bool BestRateOnly { get; set; } = true;
        public string Language { get; set; } = "EN";

        // New properties for latitude, longitude, radius, and max hotels
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public int Radius { get; set; } = 5; // Default radius value in kilometers
        public int MaxHotels { get; set; } = 10; // Default maximum number of hotels

        public HotelSearchRequest()
        {
            HotelIds = new List<string>();
        }

        public bool HasLatLng()
        {
            return Latitude != 0 && Longitude != 0;
        }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (HotelIds.Count == 0 && !HasLatLng())
            {
                yield return new ValidationResult("Either a Hotel ID or both Latitude and Longitude are required.");
            }

            // Additional custom validation logic can be added here
        }
    }
}
