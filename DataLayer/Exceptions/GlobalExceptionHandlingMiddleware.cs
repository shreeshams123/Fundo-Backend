using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
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
            if (exception is NoteException)
            {
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                var NoteCreationResponse = new
                {
                    StatusCode = HttpStatusCode.BadRequest,
                    Message = exception.Message,
                    ErrorType = "NoteException"
                };

                var NoteCreationjsonResponse = JsonSerializer.Serialize(NoteCreationResponse);
                await context.Response.WriteAsync(NoteCreationjsonResponse);
                return;
            }
            if (exception is UserException)
            {
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                var UserNotFoundResponse = new
                {
                    StatusCode = HttpStatusCode.BadRequest,
                    Message = exception.Message,
                    ErrorType = "UserException"
                };

                var UserNotFoundjsonResponse = JsonSerializer.Serialize(UserNotFoundResponse);
                await context.Response.WriteAsync(UserNotFoundjsonResponse);
                return;
            }
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            var generalresponse = new
            {
                StatusCode = HttpStatusCode.BadRequest,
                Message = "Something went wrong",
                Details = exception.Message
            };
            var generaljsonResponse = JsonSerializer.Serialize(generalresponse);
            await context.Response.WriteAsync(generaljsonResponse);
        }
    }
}
