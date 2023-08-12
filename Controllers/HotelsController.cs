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
        private readonly IAmadeusApiService _amadeusApiService;

        public HotelsController(IAmadeusApiService amadeusApiService)
        {
            _amadeusApiService = amadeusApiService;
        }

        // GET: api/Hotels
        [HttpGet]
        public async Task<ActionResult<IEnumerable<string>>> GetHotelSearchRequest([FromQuery] HotelSearchRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var responses = await _amadeusApiService.GetAmadeusResponses(request);
                return Ok(responses);
            }
            catch (Exception ex)
            {
                // Ideally, you would log the exception here
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }
}
