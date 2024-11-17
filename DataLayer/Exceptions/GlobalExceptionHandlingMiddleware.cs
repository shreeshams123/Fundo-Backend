using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;

namespace DataLayer.Exceptions
{
    public class GlobalExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionHandlingMiddleware> _logger;

        public GlobalExceptionHandlingMiddleware(RequestDelegate next, ILogger<GlobalExceptionHandlingMiddleware> logger)
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
                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            _logger.LogError(exception, "An unhandled exception has occurred.");
            context.Response.ContentType = "application/json";

            if (exception is UnauthorizedAccessException)
            {
                context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                var unauthorizedResponse = new
                {
                    StatusCode = HttpStatusCode.Unauthorized,
                    Message = "Authorization failed. Please provide a valid token.",
                    ErrorType = "UnauthorizedAccessException"
                };
                var unauthorizedJsonResponse = JsonSerializer.Serialize(unauthorizedResponse);
                await context.Response.WriteAsync(unauthorizedJsonResponse);
                return;
            }

            if (exception is NoteException)
            {
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                var noteCreationResponse = new
                {
                    StatusCode = HttpStatusCode.BadRequest,
                    Message = exception.Message,
                    ErrorType = "NoteException"
                };
                var noteCreationJsonResponse = JsonSerializer.Serialize(noteCreationResponse);
                await context.Response.WriteAsync(noteCreationJsonResponse);
                return;
            }

            if (exception is UserException)
            {
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                var userNotFoundResponse = new
                {
                    StatusCode = HttpStatusCode.BadRequest,
                    Message = exception.Message,
                    ErrorType = "UserException"
                };
                var userNotFoundJsonResponse = JsonSerializer.Serialize(userNotFoundResponse);
                await context.Response.WriteAsync(userNotFoundJsonResponse);
                return;
            }

            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            var generalResponse = new
            {
                StatusCode = HttpStatusCode.InternalServerError,
                Message = "Something went wrong. Please try again later.",
                Details = exception.Message
            };
            var generalJsonResponse = JsonSerializer.Serialize(generalResponse);
            await context.Response.WriteAsync(generalJsonResponse);
        }
    }
}
