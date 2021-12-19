using Application.Interfaces;
using Application.Wrappers;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace WebApi.Middlewares
{
    public class JwtMiddleware
    {
        private readonly RequestDelegate _next;

        public JwtMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, IAuthenticateManager authenticateService)
        {
            if (context.Request.Path.Value.ToUpper() != "/api/Authenticate/Authenticate".ToUpper())
            {
                if (context.Request.Path.Value.ToUpper() != "/api/Authenticate/CheckToken".ToUpper())
                {
                    if (!context.User.Identity.IsAuthenticated)
                    {
                        if (context.Response.HasStarted)
                        {
                            return;
                        }
                        else
                        {
                            context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                            context.Response.ContentType = "application/json; charset=utf-8";
                            var result = JsonConvert.SerializeObject(Result.Fail("Требуется авторизация"));
                            await context.Response.WriteAsync(result);
                            return;
                        }
                    }
                }
            }

            var accessToken = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

            if (accessToken is not null)
                context.Items["ActiveToken"] = await authenticateService.CheckActiveTokenAsync(accessToken);

            await _next(context);
        }
    }
}
