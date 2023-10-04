using Swashbuckle.AspNetCore.Filters;

namespace YourApiNamespace.Examples
{
    public class HotelOffersExample : IExamplesProvider<HotelOffersResponse>
    {
        public HotelOffersResponse GetExamples()
        {
            return new HotelOffersResponse // Replace with actual DTO
            {
                // Populate the fields with example data
                // ...
            };
        }
    }
}
