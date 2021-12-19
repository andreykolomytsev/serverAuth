using Microsoft.AspNetCore.Builder;
using WebApi.Middlewares;

namespace WebApi.Extensions
{
    public static class AppExtensions
    {
        /// <summary>
        /// Swagger
        /// </summary>
        /// <param name="app">Конфигурация приложения</param>
        public static void UseSwaggerExtension(this IApplicationBuilder app)
        {
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", typeof(Program).Assembly.GetName().Name);
            });
        }
        
        /// <summary>
        /// Перехватчик ошибок
        /// </summary>
        /// <param name="app">Конфигурация приложения</param>
        public static void UseErrorHandlingMiddleware(this IApplicationBuilder app)
        {
            app.UseMiddleware<ErrorHandlerMiddleware>();
        }

        /// <summary>
        /// Перехватчик JWT
        /// </summary>
        /// <param name="app">Конфигурация приложения</param>
        public static void UseJWTMiddleware(this IApplicationBuilder app)
        {
            app.UseMiddleware<JwtMiddleware>();
        }
    }
}
