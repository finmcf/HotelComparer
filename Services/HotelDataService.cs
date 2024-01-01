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
        private List<HotelOfferData> allHotelsData;

        public HotelDataService(IAmadeusApiService amadeusApiService, TestHotelDataService testHotelDataService, ILogger<HotelDataService> logger)
        {
            _amadeusApiService = amadeusApiService ?? throw new ArgumentNullException(nameof(amadeusApiService));
            _testHotelDataService = testHotelDataService ?? throw new ArgumentNullException(nameof(testHotelDataService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            allHotelsData = new List<HotelOfferData>();
        }

        public async Task<IEnumerable<HotelOfferData>> GetHotels(HotelSearchRequest request)
        {
            _logger.LogInformation($"Starting GetHotels with CheckIn: {request.CheckInDate}, CheckOut: {request.CheckOutDate}");

            if (!request.CheckInDate.HasValue || !request.CheckOutDate.HasValue)
            {
                throw new ArgumentNullException("Check-in and check-out dates are required.");
            }

            IEnumerable<string> rawResponses = request.UseTestData.HasValue && request.UseTestData.Value
                ? await _testHotelDataService.GetAllTestHotelDataAsync()
                : await _amadeusApiService.GetAmadeusResponses(request);

            allHotelsData.Clear();
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

            var groupedHotelOffers = allHotelsData.GroupBy(h => h.Hotel.HotelId)
                                                  .ToDictionary(g => g.Key, g => g.SelectMany(hotel => hotel.Offers).ToList());

            var processedHotels = new List<HotelOfferData>();
            foreach (var hotelGroup in groupedHotelOffers)
            {
                processedHotels.Add(ProcessHotelGroup(hotelGroup.Key, hotelGroup.Value, request.CheckInDate.Value, request.CheckOutDate.Value, request.Currency));
            }

            return processedHotels.Where(h => h != null).ToList();
        }

        private HotelOfferData ProcessHotelGroup(string hotelId, List<HotelOffer> offers, DateTime checkInDate, DateTime checkOutDate, string requestCurrency)
        {
            _logger.LogInformation($"Processing hotel group for {hotelId}");

            var validCombinations = GenerateAllValidCombinations(offers, checkInDate, checkOutDate);
            if (!validCombinations.Any())
            {
                _logger.LogWarning($"No valid combination found for {hotelId}");
                return null;
            }

            var cheapestCombination = validCombinations.OrderBy(c => c.TotalCost).First();
            var cheapestBasePrice = cheapestCombination.Offers.Sum(o => Convert.ToDouble(o.Price.Base));

            var hotelInfo = allHotelsData.FirstOrDefault(h => h.Hotel.HotelId == hotelId)?.Hotel;

            if (hotelInfo == null)
            {
                _logger.LogWarning($"Hotel information not found for {hotelId}");
                return null;
            }

            return new HotelOfferData
            {
                Hotel = hotelInfo,
                Offers = cheapestCombination.Offers,
                CheapestCombination = cheapestCombination.TotalCost,
                CheapestBasePrice = cheapestBasePrice, // Set the calculated cheapest base price
                CheapestCombinationCurrency = requestCurrency, // Use currency from the search request
                CheapestOfferIds = cheapestCombination.Offers.Select(o => o.Id).ToList(),
                Self = cheapestCombination.Offers.First().Self
            };
        }

        private IEnumerable<(List<HotelOffer> Offers, double TotalCost)> GenerateAllValidCombinations(List<HotelOffer> offers, DateTime checkInDate, DateTime checkOutDate)
        {
            var allCombinations = new List<(List<HotelOffer> Offers, double TotalCost)>();
            GenerateCombinationsRecursive(new List<HotelOffer>(), checkInDate, checkOutDate, offers, allCombinations);
            return allCombinations;
        }

        private void GenerateCombinationsRecursive(List<HotelOffer> currentCombination, DateTime currentDate, DateTime checkOutDate, List<HotelOffer> availableOffers, List<(List<HotelOffer> Offers, double TotalCost)> allCombinations)
        {
            if (currentDate >= checkOutDate)
            {
                double totalCost = currentCombination.Sum(o => Convert.ToDouble(o.Price.Total));
                allCombinations.Add((new List<HotelOffer>(currentCombination), totalCost));
                return;
            }

            foreach (var offer in availableOffers)
            {
                if (offer.CheckInDate <= currentDate && offer.CheckOutDate > currentDate)
                {
                    currentCombination.Add(offer);
                    var nextDate = offer.CheckOutDate > currentDate ? offer.CheckOutDate : currentDate.AddDays(1);
                    var remainingOffers = availableOffers.Where(o => !currentCombination.Contains(o)).ToList();
                    GenerateCombinationsRecursive(currentCombination, nextDate, checkOutDate, remainingOffers, allCombinations);
                    currentCombination.Remove(offer);
                }
            }
        }

        private IEnumerable<HotelOfferData> ExtractHotelsFromResponse(string jsonResponse)
        {
            _logger.LogInformation("Extracting hotels from JSON response");
            var responseObj = JsonConvert.DeserializeObject<AmadeusApiResponse>(jsonResponse);
            var hotelOfferDataList = new List<HotelOfferData>();

            foreach (var hotelData in responseObj.Data)
            {
                var hotelOfferData = new HotelOfferData
                {
                    Hotel = hotelData.Hotel,
                    Offers = new List<HotelOffer>()
                };

                // Directly use the price as is, without conversion.
                foreach (var offer in hotelData.Offers)
                {
                    hotelOfferData.Offers.Add(offer);
                }

                hotelOfferDataList.Add(hotelOfferData);
            }

            return hotelOfferDataList;
        }
    }
}
