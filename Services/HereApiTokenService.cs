using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using HotelComparer.Services;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json; // For deserializing the response
using HotelComparer.Models; // Assuming HereApiTokenResponse is in this namespace

public class HereApiTokenService : IHereApiTokenService, IDisposable
{
    private const string TokenEndpoint = "https://account.api.here.com/oauth2/token";
    private readonly IConfiguration _configuration;
    private readonly IMemoryCache _memoryCache;
    private Timer _tokenRefreshTimer;
    private const string CacheKey = "HereApiToken";

    // Declare these as private fields
    private readonly string _accessKeyId;
    private readonly string _accessKeySecret;

    public HereApiTokenService(IConfiguration configuration, IMemoryCache memoryCache)
    {
        _configuration = configuration;
        _memoryCache = memoryCache;
        _accessKeyId = _configuration["Here:AccessKeyId"];
        _accessKeySecret = _configuration["Here:AccessKeySecret"];
        StartTokenRefreshTimer();
    }

    private void StartTokenRefreshTimer()
    {
        _tokenRefreshTimer = new Timer(async _ => await FetchAndCacheTokenAsync(), null, TimeSpan.Zero, TimeSpan.FromSeconds(1790));
    }

    private async Task FetchAndCacheTokenAsync()
    {
        using var client = new HttpClient();
        var nonce = GenerateNonce();
        var timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString();
        var parameters = new Dictionary<string, string>
        {
            {"oauth_consumer_key", _accessKeyId},
            {"oauth_nonce", nonce},
            {"oauth_signature_method", "HMAC-SHA256"},
            {"oauth_timestamp", timestamp},
            {"oauth_version", "1.0"},
            {"grant_type", "client_credentials"}
        };

        var encodedParams = UrlEncodeParameters(parameters);
        var baseString = $"POST&{Uri.EscapeDataString(TokenEndpoint)}&{Uri.EscapeDataString(encodedParams)}";
        var signingKey = $"{Uri.EscapeDataString(_accessKeySecret)}&";
        var signature = GenerateSignature(baseString, signingKey);

        var authHeader = $"OAuth oauth_consumer_key=\"{Uri.EscapeDataString(_accessKeyId)}\", oauth_nonce=\"{Uri.EscapeDataString(nonce)}\", oauth_signature=\"{Uri.EscapeDataString(signature)}\", oauth_signature_method=\"HMAC-SHA256\", oauth_timestamp=\"{Uri.EscapeDataString(timestamp)}\", oauth_version=\"1.0\"";

        var request = new HttpRequestMessage(HttpMethod.Post, TokenEndpoint)
        {
            Headers = { { "Authorization", authHeader } },
            Content = new FormUrlEncodedContent(new Dictionary<string, string> { { "grant_type", "client_credentials" } })
        };

        var response = await client.SendAsync(request);
        var responseBody = await response.Content.ReadAsStringAsync();

        // Log the response body
        Console.WriteLine($"Response Body: {responseBody}");

        if (!response.IsSuccessStatusCode)
        {
            Console.WriteLine($"Error response from HERE API: {responseBody}");
            return;
        }

        var tokenResponse = JsonConvert.DeserializeObject<HereApiTokenResponse>(responseBody);
        var cacheEntryOptions = new MemoryCacheEntryOptions()
            .SetAbsoluteExpiration(TimeSpan.FromSeconds(tokenResponse?.ExpiresIn ?? 3600));
        _memoryCache.Set(CacheKey, tokenResponse?.AccessToken, cacheEntryOptions);
    }

    public async Task<string> GetAccessTokenAsync()
    {
        if (!_memoryCache.TryGetValue(CacheKey, out string token))
        {
            await FetchAndCacheTokenAsync();
            _memoryCache.TryGetValue(CacheKey, out token);
        }
        return token;
    }

    public string GetCachedAccessToken()
    {
        _memoryCache.TryGetValue(CacheKey, out string token);
        return token;
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

    public void Dispose()
    {
        _tokenRefreshTimer?.Dispose();
    }
}
