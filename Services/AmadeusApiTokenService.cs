using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using HotelComparer.Models;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace HotelComparer.Services
{
    public class AmadeusApiTokenService : IAmadeusApiTokenService
    {
        private readonly IConfiguration _configuration;
        private readonly IMemoryCache _memoryCache;
        private const string TokenEndpoint = "https://test.api.amadeus.com/v1/security/oauth2/token";
        private const string GrantType = "client_credentials";
        private const string CacheKey = "AmadeusApiToken";

        public AmadeusApiTokenService(IConfiguration configuration, IMemoryCache memoryCache)
        {
            _configuration = configuration;
            _memoryCache = memoryCache;
        }

        public async Task<string> GetAccessTokenAsync()
        {
            if (_memoryCache.TryGetValue(CacheKey, out string? token) && !string.IsNullOrEmpty(token))
            {
                return token;
            }

            using var client = new HttpClient();

            var clientId = _configuration["Amadeus:ApiKey"] ?? throw new Exception("Amadeus:ApiKey is not set in the configuration");
            var clientSecret = _configuration["Amadeus:ApiSecret"] ?? throw new Exception("Amadeus:ApiSecret is not set in the configuration");

            var request = new HttpRequestMessage(HttpMethod.Post, TokenEndpoint)
            {
                Content = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("grant_type", GrantType),
                    new KeyValuePair<string, string>("client_id", clientId),
                    new KeyValuePair<string, string>("client_secret", clientSecret)
                })
            };

            var response = await client.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Request to Amadeus API failed with status code: {response.StatusCode}");
            }

            var responseBody = await response.Content.ReadAsStringAsync();
            var tokenResponse = JsonConvert.DeserializeObject<AmadeusApiTokenResponse>(responseBody);

            if (tokenResponse == null)
            {
                throw new Exception("Failed to deserialize the token response.");
            }

            Console.WriteLine($"Response body: {responseBody}");

            var cacheEntryOptions = new MemoryCacheEntryOptions()
                // Keep in cache for this time, reset time if accessed.
                .SetAbsoluteExpiration(TimeSpan.FromSeconds(tokenResponse.ExpiresIn));

            _memoryCache.Set(CacheKey, tokenResponse.AccessToken, cacheEntryOptions);

            Console.WriteLine($"Access token: {tokenResponse.AccessToken}");

            return tokenResponse.AccessToken;
        }
    }
}
