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
                var urls = await _amadeusApiService.GenerateUrls(request);
                return Ok(urls);
            }
            catch (Exception ex)
            {
                // Log the exception here
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }
}
