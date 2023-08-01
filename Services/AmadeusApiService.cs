using System;
using System.Collections.Generic;
using HotelComparer.Models;

namespace HotelComparer.Services
{
    public class AmadeusApiService : IAmadeusApiService
    {
        private const string AMADEUS_API_URL = "https://test.api.amadeus.com/v3/shopping/hotel-offers";

        public IEnumerable<string> GenerateUrls(HotelSearchRequest request)
        {
            if (!request.CheckInDate.HasValue)
            {
                throw new ArgumentNullException(nameof(request.CheckInDate), "Check-in date cannot be null.");
            }

            if (!request.CheckOutDate.HasValue)
            {
                throw new ArgumentNullException(nameof(request.CheckOutDate), "Check-out date cannot be null.");
            }

            if (request.CheckInDate.Value >= request.CheckOutDate.Value)
            {
                throw new ArgumentException("Check-in date must be before Check-out date.");
            }

            var dateRanges = GenerateDateRanges(request.CheckInDate.Value, request.CheckOutDate.Value);
            var urls = new List<string>();
            string hotels = string.Join(",", request.HotelIds);

            foreach (var dateRange in dateRanges)
            {
                var url = $"{AMADEUS_API_URL}?hotelIds={hotels}&adults={request.Adults}&checkInDate={dateRange.Item1:yyyy-MM-dd}&checkOutDate={dateRange.Item2:yyyy-MM-dd}&countryOfResidence={request.CountryOfResidence}&roomQuantity={request.RoomQuantity}&priceRange={request.PriceRange}&currency={request.Currency}&paymentPolicy={request.PaymentPolicy}&boardType={request.BoardType}&includeClosed={request.IncludeClosed.ToString().ToLower()}&bestRateOnly={request.BestRateOnly.ToString().ToLower()}&lang={request.Language}";
                urls.Add(url);
            }

            return urls;
        }

        private static List<Tuple<DateTime, DateTime>> GenerateDateRanges(DateTime checkInDate, DateTime checkOutDate)
        {
            var dateRanges = new List<Tuple<DateTime, DateTime>>();

            for (var date = checkInDate; date < checkOutDate; date = date.AddDays(1))
            {
                for (var nextDate = date.AddDays(1); nextDate <= checkOutDate; nextDate = nextDate.AddDays(1))
                {
                    dateRanges.Add(new Tuple<DateTime, DateTime>(date, nextDate));
                }
            }

            return dateRanges;
        }
    }
}
