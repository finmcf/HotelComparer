using System.Threading.Tasks;

public interface ICurrencyApiService
{
    Task FetchCurrencyRatesAsync();
}
