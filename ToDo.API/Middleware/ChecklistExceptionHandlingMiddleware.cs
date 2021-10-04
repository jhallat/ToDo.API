using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace ToDo.API.Middleware
{
    public class ChecklistExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger _logger;

        public ChecklistExceptionHandlingMiddleware(RequestDelegate next,
            ILogger<ChecklistExceptionHandlingMiddleware> logger)
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
            catch (Exception exception)
            {
                await HandleGlobalExceptionAsync(context, exception);
            }
        }

        private Task HandleGlobalExceptionAsync(HttpContext context, Exception exception)
        {
            if (exception is KeyNotFoundException)
            {
                _logger.LogWarning("Requested resource not found. {message}", exception.Message);
                context.Response.StatusCode = (int) HttpStatusCode.NotFound;
                return context.Response.WriteAsJsonAsync(new {exception.Message});
            }

            if (exception is ApplicationException)
            {
                _logger.LogWarning("Validation error occured in API. {message}", exception.Message);
                context.Response.StatusCode = (int) HttpStatusCode.BadRequest;
                return context.Response.WriteAsJsonAsync(new {exception.Message});
            }

            var errorId = Guid.NewGuid();
            _logger.LogError(exception, "Error occured in API: {errorId}", errorId);
            context.Response.StatusCode = (int) HttpStatusCode.InternalServerError;
            return context.Response.WriteAsJsonAsync(new
            {
                ErrorId = errorId,
                Message = "An error occured on the server."
            });
        }
    }
}