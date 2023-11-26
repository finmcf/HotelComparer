using System.Threading.Tasks;

namespace HotelComparer.Services
{
    public interface IHereApiTokenService
    {
        Task<string> GetAccessTokenAsync();
    }
}
