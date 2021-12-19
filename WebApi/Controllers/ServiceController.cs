using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Application.Constants;
using Application.DTOs.MicroService;
using Application.Interfaces;
using Application.Parameters;
using Application.Wrappers;
using WebApi.Helpers;

namespace WebApi.Controllers
{
    [ApiExplorerSettings(GroupName = "Методы управления микросервисами")]
    [Route("api/[controller]")]
    [ApiController]
    public class ServiceController : ControllerBase
    {
        private readonly IServiceManager _msService;

        public ServiceController(IServiceManager msService)
        {
            _msService = msService;
        }

        /// <summary>
        /// Получить список всех микросервисов
        /// </summary>
        /// <param name="pagination">Фильтр</param>
        /// <returns></returns>
        [Authorize]
        [HttpGet]
        public async Task<ActionResult<PaginatedResult<ResponseMS>>> GetAll([FromQuery] PaginationFilter pagination)
        {
            return Ok(await _msService.GetAllMSAsync(pagination));
        }

        /// <summary>
        /// Получить микросервис
        /// </summary>
        /// <param name="msId">Id микросервиса</param>
        /// <returns></returns>
        [Authorize(AllPermissions.Services.View)]
        [HttpGet("{msId}")]
        public async Task<ActionResult<IResult<ResponseMS>>> GetById([FromRoute] string msId)
        {
            return Ok(await _msService.GetMSByIdAsync(msId));
        }

        /// <summary>
        /// Добавить новый микросервис
        /// </summary>
        /// <param name="request">Параметры микросервиса</param>
        /// <returns></returns>
        [Authorize(AllPermissions.Services.Create)]
        [HttpPost]
        public async Task<ActionResult<IResult<ResponseMS>>> Post([FromBody] RequestMS request)
        {
            return Ok(await _msService.CreateMSAsync(request));
        }

        /// <summary>
        /// Изменить микросервис
        /// </summary>
        /// <param name="request">Параметры микросервиса</param>
        /// <param name="msId">Id микросервиса для изменения</param>
        /// <returns></returns>
        [Authorize(AllPermissions.Services.Edit)]
        [HttpPut("{msId}")]
        public async Task<ActionResult<IResult<ResponseMS>>> Update([FromBody] RequestMS request, [FromRoute] string msId)
        {
            return Ok(await _msService.UpdateMSAsync(request, msId));
        }

        /// <summary>
        /// Удалить микросервис
        /// </summary>
        /// <param name="msId">Id микросервиса</param>
        /// <returns></returns>
        [Authorize(AllPermissions.Services.Delete)]
        [HttpDelete("{msId}")]
        public async Task<ActionResult<IResult<string>>> Delete([FromRoute] string msId)
        {
            return Ok(await _msService.DeleteMSAsync(msId));
        }

        /// <summary>
        /// Получить пользователей с доступом к сервису
        /// </summary>
        /// <param name="msId">Id микросервиса</param>
        /// <returns></returns>
        [Authorize(AllPermissions.Services.Edit)]
        [HttpGet("Users/{msId}")]
        public async Task<ActionResult<IResult<ResponseServiceUser>>> GetAllUsersByService([FromRoute] string msId)
        {
            return Ok(await _msService.GetAllUsersByService(msId));
        }

        /// <summary>
        /// Обновать пользователям доступ к сервису
        /// </summary>
        /// <param name="msId">Id микросервиса</param>
        /// <param name="request">Список пользователей</param>
        /// <returns></returns>
        [Authorize(AllPermissions.Services.Edit)]
        [HttpPut("Users/{msId}")]
        public async Task<ActionResult<IResult<ResponseServiceUser>>> UpdateUsersByService([FromBody] RequestServiceUser request, [FromRoute] string msId)
        {
            return Ok(await _msService.UpdateUsersByService(request, msId));
        }

        /// <summary>
        /// Получить организации с доступом к сервису
        /// </summary>
        /// <param name="msId">Id микросервиса</param>
        /// <returns></returns>
        [Authorize(AllPermissions.Services.Edit)]
        [HttpGet("Tenants/{msId}")]
        public async Task<ActionResult<IResult<ResponseServiceUser>>> GetAllTenantsByService([FromRoute] string msId)
        {
            return Ok(await _msService.GetAllTenantsByService(msId));
        }

        /// <summary>
        /// Обновать организациями доступ к сервису
        /// </summary>
        /// <param name="msId">Id микросервиса</param>
        /// <param name="request">Список организаций</param>
        /// <returns></returns>
        [Authorize(AllPermissions.Services.Edit)]
        [HttpPut("Tenants/{msId}")]
        public async Task<ActionResult<IResult<ResponseServiceUser>>> UpdateUpdateTenantsByServiceUsersByService([FromBody] RequestServiceTenant request, [FromRoute] string msId)
        {
            return Ok(await _msService.UpdateTenantsByService(request, msId));
        }
    }
}
