public interface ICurrencyExchangeService
{
    decimal CalculateExchangeRate(string baseCurrency, string targetCurrency);
}
