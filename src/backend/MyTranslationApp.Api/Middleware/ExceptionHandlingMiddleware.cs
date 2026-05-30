using MyTranslationApp.Application.DTOs;
using MyTranslationApp.Application.Exceptions;
using MyTranslationApp.Application.Validation;

namespace MyTranslationApp.Api.Middleware;

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
        catch (ProviderException provEx)
        {
            var correlationId = context.Items["CorrelationId"]?.ToString() ?? string.Empty;

            _logger.LogWarning(
                "Provider exception. CorrelationId: {CorrelationId}, ErrorCode: {ErrorCode}",
                correlationId, provEx.ErrorCode);

            context.Response.StatusCode = StatusCodes.Status502BadGateway;
            context.Response.ContentType = "application/json";

            await context.Response.WriteAsJsonAsync(new ApiErrorResponseDto
            {
                ErrorCode = provEx.ErrorCode,
                Message = provEx.Message,
                CorrelationId = correlationId
            });
        }
        catch (Exception ex)
        {
            var correlationId = context.Items["CorrelationId"]?.ToString() ?? string.Empty;

            // Log exception type only — not message or stack trace — to avoid capturing sensitive content.
            _logger.LogError(
                "Unhandled exception. CorrelationId: {CorrelationId}, ExceptionType: {ExceptionType}",
                correlationId, ex.GetType().Name);

            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            context.Response.ContentType = "application/json";

            await context.Response.WriteAsJsonAsync(new ApiErrorResponseDto
            {
                ErrorCode = ErrorCodes.InternalError,
                Message = "An unexpected error occurred. Please try again.",
                CorrelationId = correlationId
            });
        }
    }
}
