using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using HotelComparer.Models;

public class CurrencyExchangeService : ICurrencyExchangeService
{
    private readonly IMemoryCache _memoryCache;
    private readonly ILogger<CurrencyExchangeService> _logger;

    public CurrencyExchangeService(IMemoryCache memoryCache, ILogger<CurrencyExchangeService> logger)
    {
        _memoryCache = memoryCache;
        _logger = logger;
    }

    public decimal CalculateExchangeRate(string baseCurrency, string targetCurrency)
    {
        _logger.LogInformation($"Attempting to calculate exchange rate from {baseCurrency} to {targetCurrency}");

        // Convert currencies to uppercase to avoid case sensitivity issues
        baseCurrency = baseCurrency.ToUpper();
        targetCurrency = targetCurrency.ToUpper();

        if (_memoryCache.TryGetValue("CurrencyRates", out CurrencyRates rates))
        {
            _logger.LogInformation($"Currency rates retrieved from cache: {JsonConvert.SerializeObject(rates)}");

            if (rates.Rates.TryGetValue(baseCurrency, out var baseRate) &&
                rates.Rates.TryGetValue(targetCurrency, out var targetRate))
            {
                _logger.LogInformation($"Base rate for {baseCurrency}: {baseRate}");
                _logger.LogInformation($"Target rate for {targetCurrency}: {targetRate}");

                if (baseRate == 0)
                {
                    _logger.LogError($"Base rate for currency {baseCurrency} is zero.");
                    throw new InvalidOperationException($"Exchange rate for base currency '{baseCurrency}' is zero.");
                }

                var calculatedRate = targetRate / baseRate;
                _logger.LogInformation($"Calculated rate: {calculatedRate}");
                return calculatedRate;
            }

            _logger.LogError($"Exchange rate not found for currencies: {baseCurrency}, {targetCurrency}.");
            throw new KeyNotFoundException($"Exchange rate not found for one or both currencies: {baseCurrency}, {targetCurrency}.");
        }

        _logger.LogError("Currency rates not available in the cache.");
        throw new InvalidOperationException("Currency rates not available in the cache.");
    }
}
