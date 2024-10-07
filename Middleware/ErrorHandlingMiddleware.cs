using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using StudentApi.Hateoas;
using StudentApi.Models;
using System;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;

namespace StudentApi.Middleware
{
    public class ErrorHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ErrorHandlingMiddleware> _logger;

        public ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled exception occurred.");
                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            // Получение ILinkGenerator из RequestServices
            var linkGenerator = context.RequestServices.GetService(typeof(ILinkGenerator)) as ILinkGenerator;

            // Определение статуса ответа и сообщения в зависимости от типа исключения
            HttpStatusCode status;
            string message;

            switch (exception)
            {
                case ArgumentNullException _:
                case ArgumentException _:
                    status = HttpStatusCode.BadRequest;
                    message = exception.Message;
                    break;
                case KeyNotFoundException _:
                    status = HttpStatusCode.NotFound;
                    message = exception.Message;
                    break;
                default:
                    status = HttpStatusCode.InternalServerError;
                    message = "An unexpected error occurred.";
                    break;
            }

            var errorResponse = new ErrorResponse
            {
                Code = status.ToString(),
                Message = message,
                Links = linkGenerator != null ? linkGenerator.GenerateErrorLinks(status.ToString()) : new List<Link>()
            };

            var response = context.Response;
            response.ContentType = "application/json";
            response.StatusCode = (int)status;

            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
            var result = JsonSerializer.Serialize(errorResponse, options);
            await response.WriteAsync(result);
        }
    }
}
