using System;
using System.Threading.Tasks;

namespace HotelComparer.Services
{
    public interface IAmadeusApiTokenService : IDisposable
    {
        // Asynchronously gets an access token. If the token is not in the cache or expired,
        // it will fetch a new one.
        Task<string> GetAccessTokenAsync();

        // Retrieves the current access token from the cache without triggering a refresh.
        // It provides a quick way to access the token, especially for frequent read operations.
        string GetCachedAccessToken();
    }
}
