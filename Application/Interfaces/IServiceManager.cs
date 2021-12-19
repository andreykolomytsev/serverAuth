using System.Threading.Tasks;
using Application.Wrappers;
using Application.Parameters;
using Application.DTOs.MicroService;

namespace Application.Interfaces
{
    public interface IServiceManager
    {
        Task<PaginatedResult<ResponseMS>> GetAllMSAsync(PaginationFilter filter);
        Task<IResult<ResponseMS>> GetMSByIdAsync(string msId);
        Task<IResult<ResponseMS>> CreateMSAsync(RequestMS requestMS);
        Task<IResult<ResponseMS>> UpdateMSAsync(RequestMS requestMS, string msId);
        Task<IResult<string>> DeleteMSAsync(string msId);


        Task<IResult<ResponseServiceUser>> GetAllUsersByService(string msId);
        Task<IResult<ResponseServiceUser>> UpdateUsersByService(RequestServiceUser requestUsers, string msId);


        Task<IResult<ResponseServiceTenant>> GetAllTenantsByService(string msId);
        Task<IResult<ResponseServiceTenant>> UpdateTenantsByService(RequestServiceTenant requestTenants, string msId);
    }
}
