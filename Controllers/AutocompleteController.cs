using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using HotelComparer.Services;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging; // Add this to use ILogger

namespace HotelComparer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AutocompleteController : ControllerBase
    {
        private readonly IAmadeusAutocompleteService _autocompleteService;
        private readonly ILogger<AutocompleteController> _logger; // Add this field

        // Inject ILogger into the constructor
        public AutocompleteController(IAmadeusAutocompleteService autocompleteService, ILogger<AutocompleteController> logger)
        {
            _autocompleteService = autocompleteService;
            _logger = logger; // Assign the logger here
        }

        // GET: api/Autocomplete?keyword=paris
        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] string keyword)
        {
            if (string.IsNullOrWhiteSpace(keyword))
            {
                return BadRequest("Keyword is required for autocomplete.");
            }

            try
            {
                var suggestions = await _autocompleteService.GetHotelAutocompleteSuggestions(keyword);
                return Ok(suggestions);
            }
            catch (System.Exception ex)
            {
                // Log the exception with the injected logger
                _logger.LogError(ex, "An error occurred while attempting to get autocomplete suggestions for keyword {Keyword}.", keyword);

                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while processing your request.");
            }
        }
    }
}
