using Microsoft.AspNetCore.Mvc;
using HotelComparer.Services; // Ensure this is the correct namespace for your service
using System;

namespace HotelComparer.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CurrencyExchangeController : ControllerBase
    {
        private readonly ICurrencyExchangeService _currencyExchangeService;

        public CurrencyExchangeController(ICurrencyExchangeService currencyExchangeService)
        {
            _currencyExchangeService = currencyExchangeService;
        }

        [HttpGet]
        public IActionResult GetExchangeRate(string baseCurrency, string targetCurrency)
        {
            if (string.IsNullOrWhiteSpace(baseCurrency) || string.IsNullOrWhiteSpace(targetCurrency))
            {
                return BadRequest("Base and target currency codes are required.");
            }

            try
            {
                var rate = _currencyExchangeService.CalculateExchangeRate(baseCurrency, targetCurrency);
                return Ok(new { baseCurrency, targetCurrency, rate });
            }
            catch (Exception ex)
            {
                // Log the exception details
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }
    }
}
