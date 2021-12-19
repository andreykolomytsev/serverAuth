using System;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Application.Interfaces;
using Application.Wrappers;
using Domain.Settings;
using Infrastructure.Identity.Contexts;
using Infrastructure.Identity.Helpers;
using Infrastructure.Identity.Managers;

namespace Infrastructure.Identity
{
    public static class ServiceExtensions
    {
        /// <summary>
        /// Конфигурация Identity
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        public static void AddIdentityInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseNpgsql(configuration.GetConnectionString("IdentityConnection"));
                options.ConfigureWarnings(builder =>
                {
                    builder.Ignore(CoreEventId.PossibleIncorrectRequiredNavigationWithQueryFilterInteractionWarning);
                    builder.Ignore(CoreEventId.RowLimitingOperationWithoutOrderByWarning);
                });
            });

            services.AddScoped<IJwtHelper, JwtHelper>();
            services.AddScoped<IAuthenticateManager, AuthenticateManager>();
            services.AddScoped<ITokenManager, TokenManager>();           
            services.AddScoped<IUserManager, UserManager>();
            services.AddScoped<IRoleManager, RoleManager>();
            services.AddScoped<IServiceManager, ServiceManager>();
            services.AddScoped<ITenantManager, TenantManager>();
            services.AddScoped<IPermissionManager, PermissionManager>();          

            services.Configure<JWTSettings>(configuration.GetSection("JWTSettings"));

            //JWT
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
                .AddJwtBearer(o =>
                {
                    o.RequireHttpsMetadata = false;
                    o.SaveToken = false;
                    o.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ClockSkew = TimeSpan.Zero,
                        ValidIssuer = configuration["JWTSettings:Issuer"],
                        ValidAudience = configuration["JWTSettings:Audience"],
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JWTSettings:Key"]))
                    };
                    o.Events = new JwtBearerEvents()
                    {
                        OnAuthenticationFailed = context =>
                        {
                            if (context.Exception is SecurityTokenExpiredException)
                            {
                                context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                                context.Response.ContentType = "application/json; charset=utf-8";
                                var result = JsonConvert.SerializeObject(Result.Fail("Токен доступа больше неактивен"));
                                return context.Response.WriteAsync(result);
                            }
                            else if (context.Exception is SecurityTokenInvalidIssuerException || context.Exception is SecurityTokenInvalidAudienceException || context.Exception is SecurityTokenValidationException)
                            {
                                context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                                context.Response.ContentType = "application/json; charset=utf-8";
                                var result = JsonConvert.SerializeObject(Result.Fail("Некорректный токен доступа"));
                                return context.Response.WriteAsync(result);
                            }
                            else
                            {
                                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                                context.Response.ContentType = "application/json; charset=utf-8";
                                var result = JsonConvert.SerializeObject(Result.Fail("Произошла непредвиденная ошибка"));
                                return context.Response.WriteAsync(result);
                            }
                        },
                        OnChallenge = context =>
                        {
                            context.HandleResponse();
                            if (!context.Response.HasStarted)
                            {
                                context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                                context.Response.ContentType = "application/json; charset=utf-8";
                                string result = JsonConvert.SerializeObject(Result.Fail("Токен доступа больше неактивен"));
                                return context.Response.WriteAsync(result);
                            }
                            return Task.CompletedTask;
                        },
                        OnForbidden = context =>
                        {
                            context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                            context.Response.ContentType = "application/json; charset=utf-8";
                            var result = JsonConvert.SerializeObject(Result<string>.Fail("У вас нет доступа к запрашиваемому ресурсу"));
                            return context.Response.WriteAsync(result);
                        }
                    };
                });
        }

        /// <summary>
        /// Конфигурация AutoMapper
        /// </summary>
        /// <param name="services"></param>
        public static void AddApplicationLayer(this IServiceCollection services)
        {
            services.AddAutoMapper(Assembly.GetExecutingAssembly());
        }
    }
}
