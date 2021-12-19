using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Application.Constants;
using Application.DTOs.Permissions;
using Application.DTOs.Roles;
using Application.Interfaces;
using Application.Parameters;
using Application.Wrappers;
using WebApi.Helpers;

namespace WebApi.Controllers
{
    [ApiExplorerSettings(GroupName = "Методы управления ролями пользователей")]
    [Route("api/[controller]")]
    [ApiController]
    public class RoleController : ControllerBase
    {
        private readonly IRoleManager _roleService;
        private readonly IPermissionManager _permissionService;

        public RoleController(IRoleManager roleService, IPermissionManager permissionService)
        {
            _roleService = roleService;
            _permissionService = permissionService;
        }

        /// <summary>
        /// Получить список всех ролей
        /// </summary>
        /// <param name="pagination">Фильтр</param>
        /// <returns></returns>
        [Authorize(AllPermissions.Roles.View)]
        [HttpGet]
        public async Task<ActionResult<PaginatedResult<ResponseRole>>> GetAll([FromQuery] PaginationFilter pagination)
        {
            return Ok(await _roleService.GetAllRolesAsync(pagination));
        }

        /// <summary>
        /// Получить роль
        /// </summary>
        /// <param name="roleId">Id роли</param>
        /// <returns></returns>
        [Authorize(AllPermissions.Roles.View)]
        [HttpGet("{roleId}")]
        public async Task<ActionResult<IResult<ResponseRole>>> GetById([FromRoute] string roleId)
        {
            return Ok(await _roleService.GetRoleByIdAsync(roleId));
        }

        /// <summary>
        /// Добавить новую роль
        /// </summary>
        /// <param name="request">Параметры роли</param>
        /// <returns></returns>
        [Authorize(AllPermissions.Roles.Create)]
        [HttpPost]
        public async Task<ActionResult<IResult<ResponseRole>>> Post([FromBody] RequestRole request)
        {
            return Ok(await _roleService.CreateRoleAsync(request));
        }

        /// <summary>
        /// Изменить роль
        /// </summary>
        /// <param name="request">Параметры роли</param>
        /// <param name="roleId">Id роли для изменения</param>
        /// <returns></returns>
        [Authorize(AllPermissions.Roles.Edit)]
        [HttpPut("{roleId}")]
        public async Task<ActionResult<IResult<ResponseRole>>> Update([FromBody] RequestRole request, [FromRoute] string roleId)
        {
            return Ok(await _roleService.UpdateRoleAsync(request, roleId));
        }

        /// <summary>
        /// Удалить роль
        /// </summary>
        /// <param name="roleId">Id роли</param>
        /// <returns></returns>
        [Authorize(AllPermissions.Roles.Delete)]
        [HttpDelete("{roleId}")]
        public async Task<ActionResult<IResult<string>>> Delete([FromRoute] string roleId)
        {
            return Ok(await _roleService.DeleteRoleAsync(roleId));
        }

        /// <summary>
        /// Получить права доступа для роли
        /// </summary>
        /// <param name="roleId">Id роли</param>
        /// <returns></returns>
        [Authorize(AllPermissions.RolePermissions.View)]
        [HttpGet("Permissions/{roleId}")]
        public async Task<ActionResult<IResult<ResponsePermissionRole>>> GetPermissionsByRoleId([FromRoute] string roleId)
        {
            var response = await _permissionService.GetAllPermissionsByRoleIdAsync(roleId);
            return Ok(response);
        }

        /// <summary>
        /// Изменить права доступа для роли
        /// </summary>
        /// <param name="model">Параметры прав доступа</param>
        /// <param name="roleId">Id роли</param>
        /// <returns></returns>
        [Authorize(AllPermissions.RolePermissions.Edit)]
        [HttpPut("Permissions/{roleId}")]
        public async Task<ActionResult<IResult<ResponsePermissionRole>>> UpdatePermissions([FromBody] RequestPermissionRole model, [FromRoute] string roleId)
        {
            var response = await _permissionService.UpdatePermissionsForRoleAsync(model, roleId);
            return Ok(response);
        }
    }
}
