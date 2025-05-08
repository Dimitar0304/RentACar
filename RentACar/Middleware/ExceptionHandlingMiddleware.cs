using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using RentACar.Models;

namespace RentACar.Middleware
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;

        public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
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
                _logger.LogError(ex, "An unhandled exception occurred.");
                await HandleExceptionAsync(context, ex);
            }
        }

        private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            context.Response.ContentType = "text/html";

            var errorViewModel = new ErrorViewModel
            {
                RequestId = context.TraceIdentifier,
                Message = GetUserFriendlyMessage(exception)
            };

            context.Features.Set<IExceptionHandlerFeature>(new ExceptionHandlerFeature
            {
                Error = exception,
                Path = context.Request.Path
            });

            context.Response.Headers["Location"] = $"/Home/Error?message={Uri.EscapeDataString(errorViewModel.Message)}";
            await context.Response.CompleteAsync();
        }

        private static string GetUserFriendlyMessage(Exception exception)
        {
            return exception switch
            {
                ArgumentException => "Invalid input provided. Please check your data and try again.",
                InvalidOperationException => "The operation could not be completed. Please try again later.",
                UnauthorizedAccessException => "You are not authorized to perform this action.",
                _ => "An unexpected error occurred. Please try again later."
            };
        }
    }
} 