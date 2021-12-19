using System.Threading.Tasks;
using Application.DTOs.Auth;

namespace Application.Interfaces
{
    public interface IAuthenticateManager
    {
        Task<IResult<ResponseAuthentication>> AuthenticateAsync(RequestAuthentication request, string ipAddress, string browser);
        Task<IResult<ResponseAuthentication>> RefreshTokenAsync(string token, string ipAddress, string browser);
        Task<IResult<string>> RevokeRefreshTokenAsync(string token, string ipAddress);

        Task<IResult<bool>> CheckAccessTokenPermissionsAsync(string token);
        Task<bool> CheckActiveTokenAsync(string token);
    }
}
