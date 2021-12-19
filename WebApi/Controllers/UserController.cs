using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Application.Constants;
using Application.DTOs.Users;
using Application.Interfaces;
using Application.Parameters;
using Application.Wrappers;
using WebApi.Helpers;

namespace WebApi.Controllers
{
    [ApiExplorerSettings(GroupName = "Методы управления пользователями")]
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserManager _userService;

        public UserController(IUserManager userService)
        {
            _userService = userService;
        }

        /// <summary>
        /// Получить список всех пользователей
        /// </summary>
        /// <param name="filter">Фильтр</param>
        /// <param name="pagination">Постраничный вывод</param>
        /// <returns>Отфильтрованный список пользователей</returns>
        [Authorize(AllPermissions.Users.View)]
        [HttpGet]
        public async Task<ActionResult<PaginatedResult<ResponseUser>>> GetAll([FromQuery] PaginationFilter pagination, [FromQuery] UserFilter filter)
        {
            return Ok(await _userService.GetAllUsersAsync(pagination, filter));
        }

        /// <summary>
        /// Получить пользователя
        /// </summary>
        /// <param name="userId">Id пользователя</param>
        /// <returns>Запрошенный пользователь</returns>
        [Authorize(AllPermissions.Users.View)]
        [HttpGet("{userId}")]
        public async Task<ActionResult<IResult<ResponseUser>>> GetById([FromRoute] string userId)
        {
            return Ok(await _userService.GetUserByIdAsync(userId));
        }

        /// <summary>
        /// Добавить нового пользователя
        /// </summary>
        /// <param name="request">Параметры пользователя</param>
        /// <returns>Созданный пользователь</returns>
        [Authorize(AllPermissions.Users.Create)]
        [HttpPost]
        public async Task<ActionResult<IResult<ResponseUser>>> Post([FromBody] RequestCreateUser request)
        {
            return Ok(await _userService.CreateUserAsync(request));
        }

        /// <summary>
        /// Изменить пользователя
        /// </summary>
        /// <param name="userId">Id пользователя</param>
        /// <param name="request">Параметры пользователя</param>
        /// <returns>Измененный пользователь</returns>
        [Authorize(AllPermissions.Users.Edit)]
        [HttpPut("{userId}")]
        public async Task<ActionResult<IResult<ResponseUser>>> Update([FromRoute] string userId, [FromBody] RequestUpdateUser request)
        {
            return Ok(await _userService.UpdateUserAsync(request, userId));
        }

        /// <summary>
        /// Получить список ролей пользователя
        /// </summary>
        /// <param name="userId">Id пользователя</param>
        /// <returns>Список ролей пользователя</returns>
        [Authorize(AllPermissions.Users.View)]
        [HttpGet("Roles/{userId}")]
        public async Task<ActionResult<IResult<ResponseUserRoles>>> GetUserRolesAsync([FromRoute] string userId)
        {
            return Ok(await _userService.GetUserRolesAsync(userId));
        }

        /// <summary>
        /// Изменить роли пользователя
        /// </summary>
        /// <param name="userId">Id пользователя</param>
        /// <param name="request">Параметры ролей</param>
        /// <returns>Измененный список ролей пользователя</returns>
        [Authorize(AllPermissions.Users.Edit)]
        [HttpPut("Roles/{userId}")]
        public async Task<ActionResult<IResult<ResponseUserRoles>>> UpdateUserRolesAsync([FromRoute] string userId, [FromBody] RequestUserRoles request)
        {
            return Ok(await _userService.UpdateUserRolesAsync(request, userId));
        }
    }
}
