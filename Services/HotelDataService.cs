using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HotelComparer.Models;
using Newtonsoft.Json;
using Microsoft.Extensions.Logging;

namespace HotelComparer.Services
{
    public class HotelDataService : IHotelDataService
    {
        private readonly IAmadeusApiService _amadeusApiService;
        private readonly ILogger<HotelDataService> _logger;

        public HotelDataService(IAmadeusApiService amadeusApiService, ILogger<HotelDataService> logger)
        {
            _amadeusApiService = amadeusApiService ?? throw new ArgumentNullException(nameof(amadeusApiService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<IEnumerable<HotelOfferData>> GetHotels(HotelSearchRequest request)
        {
            if (!request.CheckInDate.HasValue || !request.CheckOutDate.HasValue)
            {
                throw new ArgumentNullException("Check-in and check-out dates are required.");
            }

            var rawResponses = await _amadeusApiService.GetAmadeusResponses(request);
            var allHotelsData = new List<HotelOfferData>();

            foreach (var response in rawResponses)
            {
                try
                {
                    var hotelsFromResponse = ExtractHotelsFromResponse(response);
                    allHotelsData.AddRange(hotelsFromResponse);
                }
                catch (JsonException ex)
                {
                    _logger.LogError(ex, "Error deserializing the response from Amadeus API.");
                }
            }

            var validHotelOffers = allHotelsData
                .Select(hotel =>
                {
                    var (cheapestCombination, cheapestOfferIds) = OfferCombinations.GetCheapestValidCombination(
                        hotel.Offers,
                        request.CheckInDate.Value,
                        request.CheckOutDate.Value);

                    if (cheapestCombination == -1 || cheapestOfferIds == null || !cheapestOfferIds.Any())
                    {
                        return null; // Exclude hotels without a valid combination
                    }

                    var validOffers = hotel.Offers.Where(offer => cheapestOfferIds.Contains(offer.Id)).ToList();

                    return new HotelOfferData
                    {
                        Hotel = hotel.Hotel,
                        Offers = validOffers,
                        CheapestCombination = cheapestCombination,
                        CheapestOfferIds = cheapestOfferIds,
                        Self = hotel.Self
                    };
                })
                .Where(hotel => hotel != null) // Exclude null entries
                .ToList();

            return validHotelOffers;
        }

        private IEnumerable<HotelOfferData> ExtractHotelsFromResponse(string jsonResponse)
        {
            var responseObj = JsonConvert.DeserializeObject<AmadeusApiResponse>(jsonResponse);

            // Convert currency rates
            var conversionRate = Convert.ToDouble(responseObj.Dictionaries.CurrencyConversionLookupRates["GBP"].Rate);

            // Apply conversion rate to each price in each offer
            foreach (var hotelData in responseObj.Data)
            {
                foreach (var offer in hotelData.Offers)
                {
                    offer.Price.Base = (Convert.ToDouble(offer.Price.Base) * conversionRate).ToString();
                    offer.Price.Total = (Convert.ToDouble(offer.Price.Total) * conversionRate).ToString();
                    offer.Price.Variations.Average.Base = (Convert.ToDouble(offer.Price.Variations.Average.Base) * conversionRate).ToString();
                    foreach (var change in offer.Price.Variations.Changes)
                    {
                        change.Total = (Convert.ToDouble(change.Total) * conversionRate).ToString();
                    }
                }
            }

            return responseObj.Data;
        }
    }

    public class OfferCombinations
    {
        private static (double Cost, List<string> OfferIds) FindCheapestCombination(List<HotelOffer> offers, DateTime desiredStart, DateTime desiredEnd)
        {
            int days = (desiredEnd - desiredStart).Days;
            double[,] dp = new double[days, days];
            List<string>[,] selectedOffers = new List<string>[days, days];

            for (int i = 0; i < days; i++)
            {
                for (int j = 0; j < days; j++)
                {
                    dp[i, j] = double.MaxValue;
                }
            }

            foreach (var offer in offers)
            {
                double price = Convert.ToDouble(offer.Price.Total);
                int startDayIndex = (offer.CheckInDate - desiredStart).Days;
                int endDayIndex = (offer.CheckOutDate - desiredStart).Days - 1;

                if (startDayIndex >= 0 && endDayIndex < days && dp[startDayIndex, endDayIndex] > price)
                {
                    dp[startDayIndex, endDayIndex] = price;
                    selectedOffers[startDayIndex, endDayIndex] = new List<string> { offer.Id };
                }
            }

            for (int len = 2; len <= days; len++)
            {
                for (int i = 0; i <= days - len; i++)
                {
                    int j = i + len - 1;
                    for (int k = i; k < j; k++)
                    {
                        if (dp[i, k] != double.MaxValue && dp[k + 1, j] != double.MaxValue && dp[i, j] > dp[i, k] + dp[k + 1, j])
                        {
                            dp[i, j] = dp[i, k] + dp[k + 1, j];
                            selectedOffers[i, j] = selectedOffers[i, k].Concat(selectedOffers[k + 1, j]).ToList();
                        }
                    }
                }
            }

            return (dp[0, days - 1] == double.MaxValue ? -1 : dp[0, days - 1], selectedOffers[0, days - 1]);
        }

        public static (double, List<string>) GetCheapestValidCombination(List<HotelOffer> offers, DateTime desiredStart, DateTime desiredEnd)
        {
            return FindCheapestCombination(offers, desiredStart, desiredEnd);
        }
    }
}
