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
        private readonly IAmadeusApiTokenService _amadeusApiTokenService;

        public HotelsController(IAmadeusApiService amadeusApiService, IAmadeusApiTokenService amadeusApiTokenService)
        {
            _amadeusApiService = amadeusApiService;
            _amadeusApiTokenService = amadeusApiTokenService;
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
                string token = await _amadeusApiTokenService.GetAccessTokenAsync();
                Console.WriteLine($"Access token: {token}");

                var urls = _amadeusApiService.GenerateUrls(request);
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
