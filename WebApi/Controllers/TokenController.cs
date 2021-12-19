using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Application.Constants;
using Application.DTOs.Tokens;
using Application.Enums;
using Application.Parameters;
using Application.Wrappers;
using WebApi.Helpers;
using Infrastructure.Identity.Managers;

namespace WebApi.Controllers
{
    [ApiExplorerSettings(GroupName = "Методы управления токенами")]
    [Route("api/[controller]")]
    [ApiController]
    public class TokenController : ControllerBase
    {
        private readonly ITokenManager _tokenService;

        public TokenController(ITokenManager tokenService)
        {
            _tokenService = tokenService;
        }

        /// <summary>
        /// Получить список всех токенов доступа
        /// </summary>
        /// <param name="filter">Фильтр</param>
        /// <param name="pagination">Постраничный вывод</param>
        /// <returns></returns>
        [Authorize(AllPermissions.Tokens.View)]
        [HttpGet("AccessTokens")]
        public async Task<ActionResult<PaginatedResult<ResponseRefreshToken>>> GetAllAccessTokenAsync([FromQuery] PaginationFilter pagination, [FromQuery] TokenFilter filter)
        {
            return Ok(await _tokenService.GetAllAccessTokenAsync(pagination, filter));
        }

        /// <summary>
        /// Получить список всех токенов обновления
        /// </summary>
        /// <param name="filter">Фильтр</param>
        /// <param name="pagination">Постраничный вывод</param>
        /// <returns></returns>
        [Authorize(AllPermissions.Tokens.View)]
        [HttpGet("RefreshTokens")]
        public async Task<ActionResult<PaginatedResult<ResponseRefreshToken>>> GetAllRefreshTokenAsync([FromQuery] PaginationFilter pagination, [FromQuery] TokenFilter filter)
        {
            return Ok(await _tokenService.GetAllRefreshTokenAsync(pagination, filter));
        }



        /// <summary>
        /// Активировать / Деактивировать токен доступа
        /// </summary>
        /// <param name="token">Токен доступа</param>
        /// <param name="active">Новый статус</param>
        /// <returns></returns>
        [Authorize(AllPermissions.Tokens.Edit)]
        [HttpPatch("EditAccessToken")]
        public async Task<ActionResult<Result<string>>> ChangeAccessTokenAsync([FromQuery][Required] string token, bool active)
        {
            return Ok(await _tokenService.ChangeStatusTokenAsync(token, GetIPAddress(), active));
        }

        #region Helpers
        private string GetIPAddress()
        {
            if (Request.Headers.ContainsKey("X-Forwarded-For"))
                return Request.Headers["X-Forwarded-For"];
            else
                return HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
        }
        #endregion
    }
}
