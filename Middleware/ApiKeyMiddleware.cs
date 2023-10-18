using System;
using System.Linq;
using System.Threading.Tasks;
using HotelComparer.Data;
using HotelComparer.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace HotelComparer.Middleware
{
    public class ApiKeyMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IMemoryCache _cache;

        public ApiKeyMiddleware(RequestDelegate next, IMemoryCache cache)
        {
            _next = next;
            _cache = cache;
        }

        public async Task InvokeAsync(HttpContext context, ApiDbContext dbContext)
        {
            var clientId = context.Request.Headers["ClientId"].FirstOrDefault();
            var clientSecret = context.Request.Headers["ClientSecret"].FirstOrDefault();

            if (string.IsNullOrEmpty(clientId) || string.IsNullOrEmpty(clientSecret))
            {
                context.Response.StatusCode = 401; // Unauthorized
                return;
            }

            var cacheKey = $"{clientId}-{clientSecret}";
            if (!_cache.TryGetValue(cacheKey, out ApiKey apiKey))
            {
                apiKey = await dbContext.ApiKeys
                    .Include(k => k.Permissions)
                    .FirstOrDefaultAsync(k => k.ClientId == clientId && k.ClientSecret == clientSecret);

                if (apiKey == null)
                {
                    context.Response.StatusCode = 401; // Unauthorized
                    return;
                }

                // Customize the cache options as needed
                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    // Keep in cache for this time, reset time if accessed.
                    .SetSlidingExpiration(TimeSpan.FromMinutes(2))
                    // Keep in cache for this time, regardless of access.
                    .SetAbsoluteExpiration(TimeSpan.FromHours(1));

                _cache.Set(cacheKey, apiKey, cacheEntryOptions);
            }

            // Add additional logic as needed

            await _next(context);
        }
    }
}
