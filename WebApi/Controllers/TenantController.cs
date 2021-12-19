using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Application.Constants;
using Application.DTOs.Permissions;
using Application.DTOs.Tenants;
using Application.Interfaces;
using Application.Parameters;
using Application.Wrappers;
using WebApi.Helpers;

namespace WebApi.Controllers
{
    [ApiExplorerSettings(GroupName = "Методы управления организациями")]
    [Route("api/[controller]")]
    [ApiController]
    public class TenantController : ControllerBase
    {
        private readonly ITenantManager _tenantManager;
        private readonly IPermissionManager _permissionManager;

        public TenantController(ITenantManager tenantManager, IPermissionManager permissionManager)
        {
            _tenantManager = tenantManager;
            _permissionManager = permissionManager;
        }

        /// <summary>
        /// Получить список всех организаций
        /// </summary>
        /// <param name="pagination">Фильтр</param>
        /// <returns>Отфильтрованный список организаций</returns>
        [Authorize(AllPermissions.Tenants.View)]
        [HttpGet]
        public async Task<ActionResult<PaginatedResult<ResponseTenant>>> GetAll([FromQuery] PaginationFilter pagination)
        {
            return Ok(await _tenantManager.GetAllTenantsAsync(pagination));
        }

        /// <summary>
        /// Получить организацию
        /// </summary>
        /// <param name="tenantId">Id организации</param>
        /// <returns>Запрошенная организация</returns>
        [Authorize(AllPermissions.Tenants.View)]
        [HttpGet("{tenantId}")]
        public async Task<ActionResult<IResult<ResponseTenant>>> GetById([FromRoute] string tenantId)
        {
            return Ok(await _tenantManager.GetTenantByIdAsync(tenantId));
        }

        /// <summary>
        /// Добавить новую организацию
        /// </summary>
        /// <param name="request">Параметры организации</param>
        /// <returns>Созданная организация</returns>
        [Authorize(AllPermissions.Tenants.Create)]
        [HttpPost]
        public async Task<ActionResult<IResult<ResponseTenant>>> Post([FromBody] RequestTenant request)
        {
            return Ok(await _tenantManager.CreateTenantAsync(request));
        }

        /// <summary>
        /// Изменить организацию
        /// </summary>
        /// <param name="tenantId">Id организации</param>
        /// <param name="request">Параметры организации</param>
        /// <returns>Измененная организация</returns>
        [Authorize(AllPermissions.Tenants.Edit)]
        [HttpPut("{tenantId}")]
        public async Task<ActionResult<IResult<ResponseTenant>>> Update([FromRoute] string tenantId, [FromBody] RequestTenant request)
        {
            return Ok(await _tenantManager.UpdateTenantAsync(tenantId, request));
        }

        /// <summary>
        /// Удалить организацию
        /// </summary>
        /// <param name="tenantId">Id организации</param>
        /// <returns></returns>
        [Authorize(AllPermissions.Tenants.Delete)]
        [HttpDelete("{tenantId}")]
        public async Task<ActionResult<IResult<string>>> Delete([FromRoute] string tenantId)
        {
            return Ok(await _tenantManager.DeleteTenantAsync(tenantId));
        }

        /// <summary>
        /// Получить список прав доступа для организации
        /// </summary>
        /// <param name="tenantId"></param>
        /// <returns></returns>
        [Authorize(AllPermissions.TenantPermissions.View)]
        [HttpGet("Permissions/{tenantId}")]
        public async Task<ActionResult<IResult<ResponsePermissionTenant>>> GetAllPermissionsByTenantIdAsync([FromRoute] string tenantId)
        {
            return Ok(await _permissionManager.GetAllPermissionsByTenantIdAsync(tenantId));
        }

        /// <summary>
        /// Обновить права доступа для организации
        /// </summary>
        /// <param name="tenantId">Id организации</param>
        /// <param name="request">Параметры прав доступа</param>
        /// <returns></returns>
        [Authorize(AllPermissions.TenantPermissions.Edit)]
        [HttpPut("Permissions/{tenantId}")]
        public async Task<ActionResult<IResult<ResponsePermissionTenant>>> UpdatePermissionsForTenantAsync([FromRoute] string tenantId, [FromBody] RequestPermissionTenant request)
        {
            return Ok(await _permissionManager.UpdatePermissionsForTenantAsync(request, tenantId));
        }
    }
}
