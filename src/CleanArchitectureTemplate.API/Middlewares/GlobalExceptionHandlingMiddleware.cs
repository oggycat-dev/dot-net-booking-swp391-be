using CleanArchitectureTemplate.Application.Common.DTOs;
using CleanArchitectureTemplate.Application.Common.Exceptions;
using System.Net;
using System.Text.Json;

namespace CleanArchitectureTemplate.API.Middlewares;

/// <summary>
/// Global exception handling middleware
/// </summary>
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
            _logger.LogError(ex, "An unhandled exception occurred");
            await HandleExceptionAsync(context, ex);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";
        
        var (statusCode, message, errors) = exception switch
        {
            ValidationException validationEx => (
                (int)HttpStatusCode.UnprocessableEntity,
                validationEx.Message,
                validationEx.Errors.SelectMany(e => e.Value).ToList()
            ),
            NotFoundException notFoundEx => (
                (int)HttpStatusCode.NotFound,
                notFoundEx.Message,
                new List<string> { notFoundEx.Message }
            ),
            UnauthorizedAccessException unauthorizedEx => (
                (int)HttpStatusCode.Unauthorized,
                unauthorizedEx.Message,
                new List<string> { unauthorizedEx.Message }
            ),
            ArgumentException argumentEx => (
                (int)HttpStatusCode.BadRequest,
                argumentEx.Message,
                new List<string> { argumentEx.Message }
            ),
            KeyNotFoundException keyNotFoundEx => (
                (int)HttpStatusCode.NotFound,
                "Resource not found",
                new List<string> { keyNotFoundEx.Message }
            ),
            _ => (
                (int)HttpStatusCode.InternalServerError,
                "An internal server error occurred",
                new List<string> { exception.Message }
            )
        };

        context.Response.StatusCode = statusCode;

        var response = new
        {
            StatusCode = statusCode,
            Success = false,
            Message = message,
            Data = (object?)null,
            Errors = errors,
            Timestamp = DateTime.UtcNow
        };

        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
        };

        await context.Response.WriteAsync(JsonSerializer.Serialize(response, options));
    }
}
