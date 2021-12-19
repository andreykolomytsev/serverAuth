using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Application.Constants;
using Application.DTOs.Auth;
using Application.Enums;
using Application.Interfaces;
using Application.Wrappers;
using Infrastructure.Identity.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using UAParser;
using WebApi.Helpers;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiExplorerSettings(GroupName = "Методы авторизации")]
    [ApiController]
    public class AuthenticateController : ControllerBase
    {
        private readonly IAuthenticateManager _authManager;
        private readonly IServiceManager _serviceManager;
        private readonly IJwtHelper _jwtHelper;

        public AuthenticateController(IAuthenticateManager authManager, IServiceManager serviceManager, IJwtHelper jwtHelper)
        {
            _authManager = authManager;
            _serviceManager = serviceManager;
            _jwtHelper = jwtHelper;
        }

        /// <summary>
        /// Форма авторизации
        /// </summary>
        /// <param name="request">Данные для входа</param>
        /// <returns>Токен доступа</returns>
        [Microsoft.AspNetCore.Authorization.AllowAnonymous]
        [HttpPost("Authenticate")]
        public async Task<ActionResult<Result<ResponseAuthentication>>> AuthenticateAsync([FromQuery] RequestAuthentication request)
        {
            //var resultAuth = await _authManager.AuthenticateAsync(request, GetIPAddress(), GetBrowser());

            //if (resultAuth.Succeeded && !string.IsNullOrEmpty(request.RedirectURL))
            //{
            //    var services = _jwtHelper.GetClaimsFromJwt(resultAuth.Response.AccessToken).Where(s => s.Type == AllPermissionTypes.Service).ToList();
                
            //    foreach(var service in services)
            //    {
            //        var resultService = await _serviceManager.GetMSByIdAsync(service.Value);

            //        if (!resultService.Succeeded)
            //            return Ok(resultAuth);

            //        if (resultService.Response.URL.Contains(returnURL))
            //            return Ok()
            //    }
            //}

            return Ok(await _authManager.AuthenticateAsync(request, GetIPAddress(), GetBrowser()));
        }

        /// <summary>
        /// Обновить токен обновления
        /// </summary>
        /// <param name="token">Токен обновления</param>
        /// <returns></returns>
        [Authorize]
        [HttpPost("RefreshToken")]
        public async Task<ActionResult<Result<ResponseAuthentication>>> RefreshTokenAsync([FromQuery][Required] string token)
        {
            return Ok(await _authManager.RefreshTokenAsync(token, GetIPAddress(), GetBrowser()));
        }

        /// <summary>
        /// Отозвать токен обновления
        /// </summary>
        /// <param name="token">Токен обновления</param>
        /// <returns></returns>
        [Authorize]
        [HttpPost("RevokeToken")]
        public async Task<ActionResult<Result<string>>> RevokeRefreshTokenAsync([FromQuery][Required] string token)
        {
            return Ok(await _authManager.RevokeRefreshTokenAsync(token, GetIPAddress()));
        }

        /// <summary>
        /// Проверить токен доступа
        /// </summary>
        /// <param name="token">Токен доступа</param>
        /// <returns></returns>
        [Microsoft.AspNetCore.Authorization.AllowAnonymous]
        [HttpPost("CheckToken")]
        public async Task<ActionResult<Result<bool>>> CheckTokenAsync([FromQuery][Required] string token)
        {
            return Ok(await _authManager.CheckAccessTokenPermissionsAsync(token));
        }

        
        #region Helpers
        private string GetIPAddress()
        {
            if (Request.Headers.ContainsKey("X-Forwarded-For"))
                return Request.Headers["X-Forwarded-For"];
            else
                return HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
        }

        private string GetBrowser()
        {
            var userAgent = HttpContext.Request.Headers["User-Agent"];
            string uaString = Convert.ToString(userAgent[0]);
            var uaParser = Parser.GetDefault();
            ClientInfo c = uaParser.Parse(uaString);

            return c.UA.ToString();
        }
        #endregion
    }
}