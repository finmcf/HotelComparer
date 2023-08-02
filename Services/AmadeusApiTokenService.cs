using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using HotelComparer.Models;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace HotelComparer.Services
{
    public class AmadeusApiTokenService : IAmadeusApiTokenService
    {
        private readonly IConfiguration _configuration;
        private const string TokenEndpoint = "https://test.api.amadeus.com/v1/security/oauth2/token";
        private const string GrantType = "client_credentials";

        public AmadeusApiTokenService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<string> GetAccessTokenAsync()
        {
            using var client = new HttpClient();

            var clientId = _configuration["Amadeus:ApiKey"] ?? string.Empty;
            var clientSecret = _configuration["Amadeus:ApiSecret"] ?? string.Empty;

            if (string.IsNullOrEmpty(clientId) || string.IsNullOrEmpty(clientSecret))
            {
                Console.WriteLine("Client Id or Client Secret are not provided.");
                return string.Empty;
            }

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
                Console.WriteLine($"Request to Amadeus API failed with status code: {response.StatusCode}");
                return string.Empty;
            }

            var responseBody = await response.Content.ReadAsStringAsync();
            var tokenResponse = JsonConvert.DeserializeObject<AmadeusApiTokenResponse>(responseBody);

            Console.WriteLine($"Response body: {responseBody}");

            var accessToken = tokenResponse?.AccessToken ?? string.Empty;
            Console.WriteLine($"Access token: {accessToken}");

            return accessToken;
        }
    }
}
