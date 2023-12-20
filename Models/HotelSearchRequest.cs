using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace HotelComparer.Models
{
    public class HotelSearchRequest : IValidatableObject
    {
        // Existing properties
        public List<string> HotelIds { get; set; }
        [Required]
        public DateTime? CheckInDate { get; set; }
        [Required]
        public DateTime? CheckOutDate { get; set; }

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

        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public int Radius { get; set; } = 5; // Default radius value in kilometers
        public int MaxHotels { get; set; } = 10; // Default maximum number of hotels

        // New optional parameter for using test data
        public bool? UseTestData { get; set; }

        // Constructor
        public HotelSearchRequest()
        {
            HotelIds = new List<string>();
        }

        // Helper method to check if latitude and longitude are provided
        public bool HasLatLng()
        {
            return Latitude != 0 && Longitude != 0;
        }

        // Validation logic
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            // Validation logic updated to include UseTestData
            if (HotelIds.Count == 0 && !HasLatLng() && !(UseTestData.HasValue && UseTestData.Value))
            {
                yield return new ValidationResult("Either a Hotel ID, both Latitude and Longitude, or the UseTestData flag is required.");
            }

            // Additional custom validation logic can be added here
        }
    }
}
