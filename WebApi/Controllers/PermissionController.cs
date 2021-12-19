using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Application.DTOs.Permissions;
using Application.Enums;
using Application.Wrappers;
using Application.Interfaces;
using Application.Parameters;
using WebApi.Helpers;

namespace WebApi.Controllers
{
    [ApiExplorerSettings(GroupName = "Методы управления правами доступа")]
    [Route("api/[controller]")]
    [ApiController]
    public class PermissionController : ControllerBase
    {
        private readonly IPermissionManager _permissionService;

        public PermissionController(IPermissionManager permissionService)
        {
            _permissionService = permissionService;
        }

        /// <summary>
        /// Получить список всех прав доступа
        /// </summary>
        /// <param name="pagination">Фильтр</param>
        /// <returns></returns>
        [Authorize(Roles.SuperAdmin)]
        [HttpGet]
        public async Task<ActionResult<PaginatedResult<ResponsePermission>>> GetAll([FromQuery] PaginationFilter pagination)
        {
            return Ok(await _permissionService.GetAllPermissionsAsync(pagination));
        }

        /// <summary>
        /// Получить право доступа
        /// </summary>
        /// <param name="permissionId">Id права доступа</param>
        /// <returns></returns>
        [Authorize(Roles.SuperAdmin)]
        [HttpGet("{permissionId}")]
        public async Task<ActionResult<IResult<ResponsePermission>>> GetById([FromRoute] string permissionId)
        {
            return Ok(await _permissionService.GetPermissionByIdAsync(permissionId));
        }

        /// <summary>
        /// Добавить новое право доступа
        /// </summary>
        /// <param name="request">Параметры права доступа</param>
        /// <returns></returns>
        [Authorize(Roles.SuperAdmin)]
        [HttpPost]
        public async Task<ActionResult<IResult<ResponsePermission>>> Post([FromBody] RequestPermission request)
        {
            return Ok(await _permissionService.CreatePermissionAsync(request));
        }

        /// <summary>
        /// Изменить право доступа
        /// </summary>
        /// <param name="request">Параметры права доступа</param>
        /// <param name="permissionId">Id права доступа для изменения</param>
        /// <returns></returns>
        [Authorize(Roles.SuperAdmin)]
        [HttpPut("{permissionId}")]
        public async Task<ActionResult<IResult<ResponsePermission>>> Update([FromBody] RequestPermission request, [FromRoute] string permissionId)
        {
            return Ok(await _permissionService.UpdatePermissionAsync(request, permissionId));
        }

        /// <summary>
        /// Удалить право доступа
        /// </summary>
        /// <param name="permissionId">Id права доступа</param>
        /// <returns></returns>
        [Authorize(Roles.SuperAdmin)]
        [HttpDelete("{permissionId}")]
        public async Task<ActionResult<IResult<string>>> Delete([FromRoute] string permissionId)
        {
            return Ok(await _permissionService.DeletePermissionAsync(permissionId));
        }
    }
}
