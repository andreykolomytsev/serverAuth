using Application.Exceptions;
using Application.Wrappers;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;

namespace WebApi.Middlewares
{
    public class ErrorHandlerMiddleware
    {
        private readonly RequestDelegate _next;

        public ErrorHandlerMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception error)
            {
                var response = context.Response;

                if (!response.HasStarted)
                {
                    response.ContentType = "application/json; charset=utf-8";

                    var responseModel = await Result<string>.FailAsync(error.Message);

                    response.StatusCode = error switch
                    {

                        ApiException => (int)HttpStatusCode.BadRequest,// Ошибка
                        KeyNotFoundException => (int)HttpStatusCode.NotFound,// Запись не найдена
                        _ => (int)HttpStatusCode.InternalServerError,// Ошибка сервера
                    };
                    var result = JsonSerializer.Serialize(responseModel);

                    await response.WriteAsync(result);
                }
            }
        }
    }
}
