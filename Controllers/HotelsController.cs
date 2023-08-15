using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using HotelComparer.Models;
using HotelComparer.Services;
using System.Threading.Tasks;

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
