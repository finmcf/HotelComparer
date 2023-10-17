using System.Linq;
using System.Threading.Tasks;
using HotelComparer.Data;
using HotelComparer.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace HotelComparer.Middleware
{
    public class ApiKeyMiddleware
    {
        private readonly RequestDelegate _next;

        public ApiKeyMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var clientId = context.Request.Headers["ClientId"].FirstOrDefault();
            var clientSecret = context.Request.Headers["ClientSecret"].FirstOrDefault();

            if (string.IsNullOrEmpty(clientId) || string.IsNullOrEmpty(clientSecret))
            {
                context.Response.StatusCode = 401; // Unauthorized
                return;
            }

            // Get the scoped ApiDbContext from the service provider
            var dbContext = (ApiDbContext)context.RequestServices.GetService(typeof(ApiDbContext));

            var apiKey = await dbContext.ApiKeys
                .Include(k => k.Permissions)
                .FirstOrDefaultAsync(k => k.ClientId == clientId && k.ClientSecret == clientSecret);

            if (apiKey == null)
            {
                context.Response.StatusCode = 401; // Unauthorized
                return;
            }

            await _next(context);
        }
    }
}
