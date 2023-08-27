
using System.Threading.Tasks;

namespace HotelComparer.Services
{
    public interface IAmadeusApiTokenService
    {
        Task<string> GetAccessTokenAsync();
    }
}
