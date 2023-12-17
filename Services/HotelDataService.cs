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
        private readonly TestHotelDataService _testHotelDataService;
        private readonly ILogger<HotelDataService> _logger;

        public HotelDataService(IAmadeusApiService amadeusApiService, TestHotelDataService testHotelDataService, ILogger<HotelDataService> logger)
        {
            _amadeusApiService = amadeusApiService ?? throw new ArgumentNullException(nameof(amadeusApiService));
            _testHotelDataService = testHotelDataService ?? throw new ArgumentNullException(nameof(testHotelDataService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<IEnumerable<HotelOfferData>> GetHotels(HotelSearchRequest request)
        {
            if (!request.CheckInDate.HasValue || !request.CheckOutDate.HasValue)
            {
                throw new ArgumentNullException("Check-in and check-out dates are required.");
            }

            IEnumerable<string> rawResponses;
            if (request.UseTestData.HasValue && request.UseTestData.Value)
            {
                rawResponses = await _testHotelDataService.GetAllTestHotelDataAsync();
            }
            else
            {
                rawResponses = await _amadeusApiService.GetAmadeusResponses(request);
            }

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
                    _logger.LogError(ex, "Error deserializing the response.");
                }
            }

            return allHotelsData.SelectMany(hotel => ProcessHotelData(hotel, request.CheckInDate.Value, request.CheckOutDate.Value))
                                .Where(hotel => hotel != null)
                                .ToList();
        }

        private IEnumerable<HotelOfferData> ProcessHotelData(HotelOfferData hotelData, DateTime checkInDate, DateTime checkOutDate)
        {
            var (cheapestCombination, cheapestOfferIds) = OfferCombinations.GetCheapestValidCombination(hotelData.Offers, checkInDate, checkOutDate);

            if (cheapestCombination == -1 || cheapestOfferIds == null || !cheapestOfferIds.Any())
            {
                yield return null;
            }
            else
            {
                var validOffers = hotelData.Offers.Where(offer => cheapestOfferIds.Contains(offer.Id)).ToList();

                yield return new HotelOfferData
                {
                    Hotel = hotelData.Hotel,
                    Offers = validOffers,
                    CheapestCombination = cheapestCombination,
                    CheapestOfferIds = cheapestOfferIds,
                    Self = hotelData.Self
                };
            }
        }

        private IEnumerable<HotelOfferData> ExtractHotelsFromResponse(string jsonResponse)
        {
            var responseObj = JsonConvert.DeserializeObject<AmadeusApiResponse>(jsonResponse);

            var hotelOfferDataList = new List<HotelOfferData>();
            foreach (var hotelData in responseObj.Data)
            {
                var hotelOfferData = new HotelOfferData
                {
                    Hotel = hotelData.Hotel,
                    Offers = new List<HotelOffer>()
                };

                foreach (var offer in hotelData.Offers)
                {
                    if (responseObj.Dictionaries.CurrencyConversionLookupRates.TryGetValue(offer.Price.Currency, out var conversionInfo))
                    {
                        double conversionRate = Convert.ToDouble(conversionInfo.Rate);

                        offer.Price.Base = ConvertPrice(offer.Price.Base, conversionRate);
                        offer.Price.Total = ConvertPrice(offer.Price.Total, conversionRate);
                        offer.Price.Variations.Average.Base = ConvertPrice(offer.Price.Variations.Average.Base, conversionRate);
                        foreach (var change in offer.Price.Variations.Changes)
                        {
                            change.Total = ConvertPrice(change.Total, conversionRate);
                        }
                    }
                    else
                    {
                        _logger.LogWarning($"Conversion rate for currency '{offer.Price.Currency}' not found. Skipping conversion.");
                    }

                    hotelOfferData.Offers.Add(offer);
                }

                hotelOfferDataList.Add(hotelOfferData);
            }

            return hotelOfferDataList;
        }

        private string ConvertPrice(string price, double rate)
        {
            return (Convert.ToDouble(price) * rate).ToString("F2");
        }
    }

    public static class OfferCombinations
    {
        public static (double Cost, List<string> OfferIds) GetCheapestValidCombination(List<HotelOffer> offers, DateTime desiredStart, DateTime desiredEnd)
        {
            return FindCheapestCombination(offers, desiredStart, desiredEnd);
        }

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
    }
}
