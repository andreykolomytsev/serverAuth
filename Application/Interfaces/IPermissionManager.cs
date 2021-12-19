using System.Threading.Tasks;
using Application.Wrappers;
using Application.Parameters;
using Application.DTOs.Permissions;

namespace Application.Interfaces
{
    public interface IPermissionManager
    {
        Task<PaginatedResult<ResponsePermission>> GetAllPermissionsAsync(PaginationFilter filter);
        Task<IResult<ResponsePermission>> GetPermissionByIdAsync(string permissionId);
        Task<IResult<ResponsePermission>> CreatePermissionAsync(RequestPermission request);
        Task<IResult<ResponsePermission>> UpdatePermissionAsync(RequestPermission request, string permissionId);
        Task<IResult<string>> DeletePermissionAsync(string permissionId);


        Task<IResult<ResponsePermissionRole>> GetAllPermissionsByRoleIdAsync(string roleId);
        Task<IResult<ResponsePermissionRole>> UpdatePermissionsForRoleAsync(RequestPermissionRole request, string roleId);


        Task<IResult<ResponsePermissionTenant>> GetAllPermissionsByTenantIdAsync(string tenantId);
        Task<IResult<ResponsePermissionTenant>> UpdatePermissionsForTenantAsync(RequestPermissionTenant request, string tenantId);
    }
}
