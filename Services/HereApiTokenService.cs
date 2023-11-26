﻿using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using HotelComparer.Models; // Importing the Models namespace

namespace HotelComparer.Services
{
    public class HereApiTokenService : IHereApiTokenService, IDisposable
    {
        private readonly IConfiguration _configuration;
        private readonly IMemoryCache _memoryCache;
        private Timer _tokenRefreshTimer;
        private const string TokenEndpoint = "https://account.api.here.com/oauth2/token";
        private const string CacheKey = "HereApiToken";

        public HereApiTokenService(IConfiguration configuration, IMemoryCache memoryCache)
        {
            _configuration = configuration;
            _memoryCache = memoryCache;
            StartTokenRefreshTimer();
        }

        private void StartTokenRefreshTimer()
        {
            // Refresh the token immediately on startup
            _tokenRefreshTimer = new Timer(async _ => await RefreshTokenAsync(), null, TimeSpan.Zero, Timeout.InfiniteTimeSpan);
        }

        private async Task RefreshTokenAsync()
        {
            try
            {
                using var client = new HttpClient();
                var accessKeyId = _configuration["Here:AccessKeyId"] ?? throw new Exception("Here:AccessKeyId is not set in the configuration");
                var accessKeySecret = _configuration["Here:AccessKeySecret"] ?? throw new Exception("Here:AccessKeySecret is not set in the configuration");

                var nonce = GenerateNonce();
                var timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString();

                var parameters = new Dictionary<string, string>
                {
                    {"oauth_consumer_key", accessKeyId},
                    {"oauth_nonce", nonce},
                    {"oauth_signature_method", "HMAC-SHA256"},
                    {"oauth_timestamp", timestamp},
                    {"oauth_version", "1.0"},
                    {"grant_type", "client_credentials"}
                };

                var encodedParams = UrlEncodeParameters(parameters);
                var baseString = $"POST&{Uri.EscapeDataString(TokenEndpoint)}&{Uri.EscapeDataString(encodedParams)}";
                var signingKey = $"{Uri.EscapeDataString(accessKeySecret)}&";
                var signature = GenerateSignature(baseString, signingKey);

                var authHeader = $"OAuth oauth_consumer_key=\"{Uri.EscapeDataString(accessKeyId)}\", oauth_nonce=\"{Uri.EscapeDataString(nonce)}\", oauth_signature=\"{Uri.EscapeDataString(signature)}\", oauth_signature_method=\"HMAC-SHA256\", oauth_timestamp=\"{Uri.EscapeDataString(timestamp)}\", oauth_version=\"1.0\"";

                var request = new HttpRequestMessage(HttpMethod.Post, TokenEndpoint)
                {
                    Headers = { { "Authorization", authHeader } },
                    Content = new FormUrlEncodedContent(new Dictionary<string, string> { { "grant_type", "client_credentials" } })
                };

                var response = await client.SendAsync(request);
                var responseBody = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"Error response from HERE API: {responseBody}");
                    throw new Exception($"Request to HERE API failed with status code: {response.StatusCode}");
                }

                Console.WriteLine($"Received token response: {responseBody}");

                var tokenResponse = JsonConvert.DeserializeObject<HereApiTokenResponse>(responseBody);

                if (tokenResponse == null || tokenResponse.ExpiresIn <= 0)
                {
                    throw new Exception("Invalid token response or negative expiration time.");
                }

                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    .SetAbsoluteExpiration(TimeSpan.FromSeconds(tokenResponse.ExpiresIn));

                _memoryCache.Set(CacheKey, tokenResponse.AccessToken, cacheEntryOptions);

                // Schedule the next refresh just before the token expires
                var nextRefreshTime = TimeSpan.FromSeconds(tokenResponse.ExpiresIn - 30); // 30 seconds before expiration
                _tokenRefreshTimer.Change(nextRefreshTime, Timeout.InfiniteTimeSpan);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error while refreshing token: {ex.Message}");
            }
        }

        private static string UrlEncodeParameters(Dictionary<string, string> parameters)
        {
            var sortedParams = new SortedDictionary<string, string>(parameters);
            var encodedParams = new StringBuilder();

            foreach (var param in sortedParams)
            {
                if (encodedParams.Length > 0)
                {
                    encodedParams.Append("&");
                }
                encodedParams.Append(Uri.EscapeDataString(param.Key));
                encodedParams.Append("=");
                encodedParams.Append(Uri.EscapeDataString(param.Value));
            }

            return encodedParams.ToString();
        }

        private static string GenerateNonce()
        {
            return Guid.NewGuid().ToString("N");
        }

        private static string GenerateSignature(string data, string key)
        {
            using var encoder = new HMACSHA256(Encoding.UTF8.GetBytes(key));
            var hash = encoder.ComputeHash(Encoding.UTF8.GetBytes(data));
            return Convert.ToBase64String(hash);
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

        public void Dispose()
        {
            _tokenRefreshTimer?.Dispose();
        }
    }
}
