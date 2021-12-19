using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using System;
using System.IO;
using System.Reflection;

namespace WebApi.Extensions
{
    public static class ServiceExtensions
    {
        /// <summary>
        /// Конфигурация Swagger
        /// </summary>
        /// <param name="services">Спецификация сервиса</param>
        public static void AddSwaggerExtension(this IServiceCollection services)
        {
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "authService",
                    Description = "Сервис реализует методы авторизации, управления пользователями и настройки доступа пользователей к микросервисам",
                });

                //Группируем по тегу
                c.TagActionsBy(api =>
                {
                    if (api.GroupName != null)
                    {
                        return new[] { api.GroupName };
                    }

                    ControllerActionDescriptor controllerActionDescriptor = (ControllerActionDescriptor)api.ActionDescriptor;
                    if (controllerActionDescriptor != null)
                    {
                        return new[] { controllerActionDescriptor.ControllerName };
                    }

                    throw new InvalidOperationException("Некорректный тег для конечной точки");
                });

                c.DocInclusionPredicate((name, api) => true);

                ////Порядок отображения запросов
                //string[] methodsOrder = new string[5] { "POST", "GET", "PUT", "DELETE", "PATCH" };
                //c.OrderActionsBy(apiDesc => $"{apiDesc.ActionDescriptor.RouteValues["controller"]}_{Array.IndexOf(methodsOrder, apiDesc.HttpMethod)}");

                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);

                c.IncludeXmlComments(xmlPath);

                var jwtSecurityScheme = new OpenApiSecurityScheme
                {
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    Name = "JWT Authentication",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.Http,
                    Description = "Укажите Ваш JWT token в поле ниже",
                    Reference = new OpenApiReference
                    {
                        Id = JwtBearerDefaults.AuthenticationScheme,
                        Type = ReferenceType.SecurityScheme
                    }
                };

                c.AddSecurityDefinition(jwtSecurityScheme.Reference.Id, jwtSecurityScheme);
                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    { jwtSecurityScheme, Array.Empty<string>() }
                 });
            });
        }

        /// <summary>
        /// Конфигурация версий API 
        /// </summary>
        /// <param name="services">Спецификация сервиса</param>
        public static void AddApiVersioningExtension(this IServiceCollection services)
        {
            services.AddApiVersioning(config =>
            {
                // Спецификация API версия 1.0
                config.DefaultApiVersion = new ApiVersion(1, 0);
                // Если не указана версия API в запросе, используем версию по умолчанию
                config.AssumeDefaultVersionWhenUnspecified = true;
                // Версия API, поддерживаемая для конкретной конечной точки
                config.ReportApiVersions = true;
            });
        }
    }
}
