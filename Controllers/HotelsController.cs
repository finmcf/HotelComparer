using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using HotelComparer.Models;
using HotelComparer.Services;
using System.Threading.Tasks;
using Swashbuckle.AspNetCore.Filters;  // Make sure to add this line

namespace HotelComparer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HotelsController : ControllerBase
    {
        private readonly IHotelDataService _hotelDataService;

        public HotelsController(IHotelDataService hotelDataService)
        {
            _hotelDataService = hotelDataService;
        }

        // GET: api/Hotels
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<HotelOfferData>))]
        [SwaggerResponseExample(StatusCodes.Status200OK, typeof(HotelOfferDataExample))]  // Added this line
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<HotelOfferData>>> GetHotels([FromQuery] HotelSearchRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var hotelOffers = await _hotelDataService.GetHotels(request);
                return Ok(hotelOffers);
            }
            catch (Exception ex)
            {
                // Ideally, you would log the exception here
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }
}
