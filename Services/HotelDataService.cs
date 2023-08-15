using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HotelComparer.Models;
using Newtonsoft.Json;

namespace HotelComparer.Services
{
    public class HotelDataService : IHotelDataService
    {
        private readonly IAmadeusApiService _amadeusApiService;

        public HotelDataService(IAmadeusApiService amadeusApiService)
        {
            _amadeusApiService = amadeusApiService ?? throw new ArgumentNullException(nameof(amadeusApiService));
        }

        public async Task<IEnumerable<HotelOfferData>> GetHotels(HotelSearchRequest request)
        {
            var rawResponses = await _amadeusApiService.GetAmadeusResponses(request);
            var allHotelsData = new List<HotelOfferData>();

            foreach (var response in rawResponses)
            {
                try
                {
                    var hotelsFromResponse = ExtractHotelsFromResponse(response);
                    allHotelsData.AddRange(hotelsFromResponse);
                }
                catch (JsonException) // Handle possible deserialization issues
                {
                    // Handle or log the error.
                    // Maybe you want to continue or halt execution based on the nature of your application.
                }
            }

            // Group by hotel data
            var groupedHotelOffers = allHotelsData.GroupBy(h => h.Hotel.Id).Select(group => new HotelOfferData
            {
                Hotel = group.First().Hotel,
                Offers = group.SelectMany(g => g.Offers).ToList()
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
                    Id = data.Hotel.HotelId,
                    Name = data.Hotel.Name,
                    ChainCode = data.Hotel.ChainCode,
                    CityCode = data.Hotel.CityCode,
                    Latitude = data.Hotel.Latitude,
                    Longitude = data.Hotel.Longitude
                },
                Offers = data.Offers.Select(offer => new HotelOffer
                {
                    Id = offer.Id,
                    CheckInDate = DateTime.Parse(offer.CheckInDate),
                    CheckOutDate = DateTime.Parse(offer.CheckOutDate),
                    Price = Convert.ToDouble(offer.Price.Base) * conversionRate,
                    Currency = responseObj.Dictionaries.CurrencyConversionLookupRates["GBP"].Target,
                    RoomType = offer.Room.TypeEstimated.Category,
                    BedType = offer.Room.TypeEstimated.BedType,
                    Description = offer.Room.Description.Text
                }).ToList()
            }).ToList();
        }
    }
}
