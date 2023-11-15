using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using HotelComparer.Models;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace HotelComparer.Services
{
    public class AmadeusApiTokenService : IAmadeusApiTokenService, IDisposable
    {
        private readonly IConfiguration _configuration;
        private readonly IMemoryCache _memoryCache;
        private Timer _tokenRefreshTimer;
        private const string TokenEndpoint = "https://test.api.amadeus.com/v1/security/oauth2/token";
        private const string GrantType = "client_credentials";
        private const string CacheKey = "AmadeusApiToken";

        public AmadeusApiTokenService(IConfiguration configuration, IMemoryCache memoryCache)
        {
            _configuration = configuration;
            _memoryCache = memoryCache;
            StartTokenRefreshTimer();
        }

        private void StartTokenRefreshTimer()
        {
            // Set the timer to trigger immediately and then every 1790 seconds
            _tokenRefreshTimer = new Timer(async _ => await RefreshTokenAsync(), null, TimeSpan.Zero, TimeSpan.FromSeconds(1790));
        }

        private async Task RefreshTokenAsync()
        {
            try
            {
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
                Console.WriteLine($"Received token response: {responseBody}"); // Log the response

                var tokenResponse = JsonConvert.DeserializeObject<AmadeusApiTokenResponse>(responseBody);

                if (tokenResponse == null)
                {
                    throw new Exception("Failed to deserialize the token response.");
                }

                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    .SetAbsoluteExpiration(TimeSpan.FromSeconds(tokenResponse.ExpiresIn));

                _memoryCache.Set(CacheKey, tokenResponse.AccessToken, cacheEntryOptions);
            }
            catch (Exception ex)
            {
                // Log the exception or handle it as per your application's error handling strategy
                Console.WriteLine($"Error while refreshing token: {ex.Message}");
            }
        }

        public async Task<string> GetAccessTokenAsync()
        {
            if (!_memoryCache.TryGetValue(CacheKey, out string token))
            {
                await RefreshTokenAsync();
                _memoryCache.TryGetValue(CacheKey, out token);
            }

            return token;
        }

        public string GetCachedAccessToken()
        {
            _memoryCache.TryGetValue(CacheKey, out string token);
            return token;
        }

        public void Dispose()
        {
            _tokenRefreshTimer?.Dispose();
        }
    }
}
