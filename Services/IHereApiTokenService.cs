using System.Threading.Tasks;

namespace HotelComparer.Services
{
    public interface IHereApiTokenService
    {
        Task<string> GetAccessTokenAsync(); // Method to get the access token, either from cache or by fetching a new one
        string GetCachedAccessToken(); // Method to directly get the access token from the cache
    }
}
