using Application.DTOs.Users;
using Application.Parameters;
using Application.Wrappers;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IUserManager
    {
        Task<PaginatedResult<ResponseUser>> GetAllUsersAsync(PaginationFilter pagination, UserFilter filter);
        Task<IResult<ResponseUser>> GetUserByIdAsync(string userId);
        Task<IResult<ResponseUser>> CreateUserAsync(RequestCreateUser request);
        Task<IResult<ResponseUser>> UpdateUserAsync(RequestUpdateUser request, string userId);
        Task<IResult<ResponseUser>> DeleteUserAsync(string userId);


        Task<IResult<ResponseUserRoles>> GetUserRolesAsync(string userId);
        Task<IResult<ResponseUserRoles>> UpdateUserRolesAsync(RequestUserRoles request, string userId);
    }
}
