using Application.DTOs.Permissions;
using Application.DTOs.Roles;
using Application.Parameters;
using Application.Wrappers;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IRoleManager
    {
        Task<PaginatedResult<ResponseRole>> GetAllRolesAsync(PaginationFilter filter);
        Task<IResult<ResponseRole>> GetRoleByIdAsync(string roleId);
        Task<IResult<ResponseRole>> CreateRoleAsync(RequestRole request);
        Task<IResult<ResponseRole>> UpdateRoleAsync(RequestRole request, string roleId);  
        Task<IResult<string>> DeleteRoleAsync(string roleId);


        //Task<IResult<ResponsePermissionRole>> GetPermissionsByRoleAsync(string roleId);          
        //Task<IResult<string>> UpdatePermissionsByRoleAsync(RequestPermissionRole request, string roleId);
    }
}
