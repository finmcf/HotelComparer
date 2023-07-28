using HotelPriceComparer.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace HotelPriceComparer.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class HotelsController : ControllerBase
    {
        private readonly IHttpClientFactory _clientFactory;
        private readonly IConfiguration _configuration;

        public HotelsController(IHttpClientFactory clientFactory, IConfiguration configuration)
        {
            _clientFactory = clientFactory;
            _configuration = configuration;
        }

        [HttpPost("search")]
        public async Task<ActionResult<HotelSearchResult>> Post(HotelSearchRequest request)
        {
            var cheapestBooking = new HotelSearchResult();

            // Get all possible date combinations for the given range
            var dateCombinations = GetDateCombinations(request.StartDate, request.EndDate);

            foreach (var dateCombination in dateCombinations)
            {
                var bookingOptionForCombination = new HotelSearchResult();

                foreach (var dates in dateCombination)
                {
                    // Call the Amadeus API for each date
                    var result = await SearchAmadeusHotels(request.Location, dates.Item1, dates.Item2);

                    if (result != null)
                    {
                        var bookingOption = new HotelBookingOption
                        {
                            HotelName = result.HotelName, // You would need to fetch hotel name from API response
                            CheckInDate = dates.Item1,
                            CheckOutDate = dates.Item2,
                            Price = result.TotalPrice
                        };
                        bookingOptionForCombination.Results.Add(bookingOption);
                    }
                }

                if (bookingOptionForCombination.Results.Any() && bookingOptionForCombination.TotalPrice < cheapestBooking.TotalPrice)
                {
                    cheapestBooking = bookingOptionForCombination;
                }
            }

            if (!cheapestBooking.Results.Any())
            {
                // No valid result was found, return an appropriate response
                return NotFound("No valid hotel booking option was found.");
            }

            return cheapestBooking;
        }

        private List<List<Tuple<DateTime, DateTime>>> GetDateCombinations(DateTime start, DateTime end)
        {
            // This method should return all possible combinations of dates within the given range
            throw new NotImplementedException();
        }

        private async Task<HotelSearchResult?> SearchAmadeusHotels(string location, DateTime checkInDate, DateTime checkOutDate)
        {
            var apiKey = _configuration["Amadeus:ApiKey"];
            var apiSecret = _configuration["Amadeus:ApiSecret"];

            var request = new HttpRequestMessage(HttpMethod.Get,
                $"https://api.amadeus.com/v2/shopping/hotel-offers?cityCode={location}&checkInDate={checkInDate:yyyy-MM-dd}&checkOutDate={checkOutDate:yyyy-MM-dd}");

            request.Headers.Add("Authorization", $"Bearer {apiKey}:{apiSecret}");
            request.Headers.Add("Accept", "application/json");

            var client = _clientFactory.CreateClient();
            var response = await client.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                var responseStream = await response.Content.ReadAsStreamAsync();
                var searchResult = await JsonSerializer.DeserializeAsync<HotelSearchResult>(responseStream);

                return searchResult;
            }
            else
            {
                // Handle errors here
                throw new Exception($"Error fetching data from Amadeus API: {response.StatusCode}");
            }
        }
    }
}
