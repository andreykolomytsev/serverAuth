using Application.DTOs.Tenants;
using Application.DTOs.Users;
using Application.Parameters;
using Application.Wrappers;
using System;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface ITenantManager
    {
        Task<PaginatedResult<ResponseTenant>> GetAllTenantsAsync(PaginationFilter filter);

        Task<IResult<ResponseTenant>> GetTenantByIdAsync(string tenantId);

        Task<IResult<ResponseTenant>> CreateTenantAsync(RequestTenant requestTenant);

        Task<IResult<ResponseTenant>> UpdateTenantAsync(string tenantId, RequestTenant requestTenant);

        Task<IResult<string>> DeleteTenantAsync(string tenantId);
    }
}
