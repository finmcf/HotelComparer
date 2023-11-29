using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using HotelComparer.Services;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace HotelComparer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AutocompleteController : ControllerBase
    {
        private readonly ICombinedResponsesService _combinedService;
        private readonly ILogger<AutocompleteController> _logger;

        public AutocompleteController(ICombinedResponsesService combinedService, ILogger<AutocompleteController> logger)
        {
            _combinedService = combinedService;
            _logger = logger;
        }

        
        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] string keyword, [FromQuery] double? latitude, [FromQuery] double? longitude)
        {
            if (string.IsNullOrWhiteSpace(keyword))
            {
                return BadRequest("Keyword is required for autocomplete.");
            }

            try
            {
                var suggestions = await _combinedService.GetCombinedSuggestions(keyword, latitude, longitude);
                return Ok(suggestions);
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "An error occurred while attempting to get combined autocomplete suggestions for keyword {Keyword}.", keyword);
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while processing your request.");
            }
        }
    }
}
