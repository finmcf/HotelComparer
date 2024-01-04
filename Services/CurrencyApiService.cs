using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using HotelComparer.Models; // Assuming CurrencyRates model is in this namespace

public class CurrencyApiService : ICurrencyApiService, IDisposable
{
    private readonly HttpClient _httpClient;
    private readonly IMemoryCache _memoryCache;
    private readonly string _apiKey;
    private Timer _fetchRatesTimer;

    public CurrencyApiService(HttpClient httpClient, IMemoryCache memoryCache, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _memoryCache = memoryCache;
        _apiKey = configuration["CurrencyApi:ApiKey"];
        StartFetchRatesTimer();
    }

    private void StartFetchRatesTimer()
    {
        // Start the timer to fetch rates immediately and then at every one-hour interval
        _fetchRatesTimer = new Timer(async _ => await FetchCurrencyRatesAsync(), null, TimeSpan.Zero, TimeSpan.FromHours(1));
    }

    public async Task FetchCurrencyRatesAsync()
    {
        var response = await _httpClient.GetAsync($"https://currencyapi.net/api/v1/rates?key={_apiKey}&output=json");
        if (response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"Received response: {content}"); // Logging the response
            var rates = JsonConvert.DeserializeObject<CurrencyRates>(content);
            _memoryCache.Set("CurrencyRates", rates, TimeSpan.FromHours(1));
        }
        else
        {
            Console.WriteLine($"Error fetching rates: {response.StatusCode}");
        }
    }

    public void Dispose()
    {
        _fetchRatesTimer?.Dispose();
    }
}
