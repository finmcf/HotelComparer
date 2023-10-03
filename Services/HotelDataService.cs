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

            var groupedHotelOffers = allHotelsData.GroupBy(h => h.Hotel.HotelId).Select(group =>
            {
                var offers = group.SelectMany(g => g.Offers).ToList();
                var (cheapestCombination, cheapestOfferIds) = OfferCombinations.GetCheapestValidCombination(
                    offers,
                    request.CheckInDate.Value,
                    request.CheckOutDate.Value);

                return new HotelOfferData
                {
                    Hotel = group.First().Hotel,
                    Offers = offers,
                    CheapestCombination = cheapestCombination,
                    CheapestOfferIds = cheapestOfferIds,
                    Self = group.First().Self
                };
            }).ToList();

            return groupedHotelOffers;
        }

        private IEnumerable<HotelOfferData> ExtractHotelsFromResponse(string jsonResponse)
        {
            var responseObj = JsonConvert.DeserializeObject<AmadeusApiResponse>(jsonResponse);
            var conversionRate = Convert.ToDouble(responseObj.Dictionaries.CurrencyConversionLookupRates["GBP"].Rate);

            return responseObj.Data.Select(data => new HotelOfferData
            {
                Hotel = new HotelInfo
                {
                    HotelId = data.Hotel.HotelId,
                    Name = data.Hotel.Name,
                    ChainCode = data.Hotel.ChainCode,
                    CityCode = data.Hotel.CityCode,
                    Latitude = data.Hotel.Latitude,
                    Longitude = data.Hotel.Longitude
                },
                Self = data.Self,
                Offers = data.Offers.Select(offer => new HotelOffer
                {
                    Id = offer.Id,
                    CheckInDate = offer.CheckInDate,
                    CheckOutDate = offer.CheckOutDate,
                    RateCode = offer.RateCode,
                    Room = new HotelRoom
                    {
                        Type = offer.Room.Type,
                        TypeEstimated = new TypeEstimated
                        {
                            Category = offer.Room.TypeEstimated.Category,
                            Beds = offer.Room.TypeEstimated.Beds,
                            BedType = offer.Room.TypeEstimated.BedType
                        },
                        Description = new RoomDescription
                        {
                            Text = offer.Room.Description.Text,
                            Lang = offer.Room.Description.Lang
                        }
                    },
                    Guests = new GuestInfo
                    {
                        Adults = offer.Guests.Adults
                    },
                    Price = new HotelPrice
                    {
                        Currency = offer.Price.Currency,
                        Base = (Convert.ToDouble(offer.Price.Base) * conversionRate).ToString(),
                        Total = (Convert.ToDouble(offer.Price.Total) * conversionRate).ToString(),
                        Variations = new PriceVariations
                        {
                            Average = new AveragePrice
                            {
                                Base = (Convert.ToDouble(offer.Price.Variations.Average.Base) * conversionRate).ToString()
                            },
                            Changes = offer.Price.Variations.Changes.Select(change => new PriceChange
                            {
                                StartDate = change.StartDate,
                                EndDate = change.EndDate,
                                Total = (Convert.ToDouble(change.Total) * conversionRate).ToString()
                            }).ToList()
                        }
                    },
                    Policies = new PolicyInfo
                    {
                        PaymentType = offer.Policies.PaymentType,
                        Cancellations = offer.Policies.Cancellations.Select(policy => new CancellationPolicy
                        {
                            Amount = (Convert.ToDouble(policy.Amount) * conversionRate).ToString(),
                            Deadline = policy.Deadline
                        }).ToList()
                    },
                    Self = offer.Self
                }).ToList()
            }).ToList();
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
                for (int j = 0; j < days; j++)
                    dp[i, j] = double.MaxValue;

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
