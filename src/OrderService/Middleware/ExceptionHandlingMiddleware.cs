using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Serialization;
using FluentValidation;
using OrderService.Application.DTOs;
using OrderService.Domain.Enums;

namespace OrderService.Middleware;

/// <summary>
/// Global exception handling middleware.
/// Catches all unhandled exceptions and converts them to standardized error responses.
/// </summary>
public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        Converters = { new JsonStringEnumConverter() }
    };

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
        catch (ValidationException ex)
        {
            await HandleValidationExceptionAsync(context, ex);
        }
        catch (KeyNotFoundException ex)
        {
            await HandleNotFoundExceptionAsync(context, ex);
        }
        catch (InvalidOperationException ex)
        {
            await HandleInvalidOperationExceptionAsync(context, ex);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleValidationExceptionAsync(HttpContext context, ValidationException exception)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = StatusCodes.Status400BadRequest;

        var traceId = Activity.Current?.Id ?? context.TraceIdentifier;

        var errorResponse = new ErrorResponse
        {
            TraceId = traceId,
            Code = ErrorCode.VALIDATION_ERROR,
            Message = "One or more validation errors occurred",
            Errors = exception.Errors.Select(e => new ValidationError
            {
                Field = e.PropertyName,
                Message = e.ErrorMessage
            }).ToList()
        };

        _logger.LogWarning("Validation error: {TraceId}", traceId);

        await context.Response.WriteAsync(JsonSerializer.Serialize(errorResponse, JsonOptions));
    }

    private async Task HandleNotFoundExceptionAsync(HttpContext context, KeyNotFoundException exception)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = StatusCodes.Status404NotFound;

        var traceId = Activity.Current?.Id ?? context.TraceIdentifier;

        var errorResponse = new ErrorResponse
        {
            TraceId = traceId,
            Code = ErrorCode.ORDER_NOT_FOUND,
            Message = exception.Message
        };

        _logger.LogWarning("Resource not found: {TraceId} - {Message}", traceId, exception.Message);

        await context.Response.WriteAsync(JsonSerializer.Serialize(errorResponse, JsonOptions));
    }

    private async Task HandleInvalidOperationExceptionAsync(HttpContext context, InvalidOperationException exception)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = StatusCodes.Status400BadRequest;

        var traceId = Activity.Current?.Id ?? context.TraceIdentifier;

        var errorResponse = new ErrorResponse
        {
            TraceId = traceId,
            Code = ErrorCode.INVALID_STATUS_TRANSITION,
            Message = exception.Message
        };

        _logger.LogWarning("Invalid operation: {TraceId} - {Message}", traceId, exception.Message);

        await context.Response.WriteAsync(JsonSerializer.Serialize(errorResponse, JsonOptions));
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = StatusCodes.Status500InternalServerError;

        var traceId = Activity.Current?.Id ?? context.TraceIdentifier;

        var errorResponse = new ErrorResponse
        {
            TraceId = traceId,
            Code = ErrorCode.INTERNAL_ERROR,
            Message = "An unexpected error occurred",
            Details = exception.Message
        };

        _logger.LogError(exception, "Internal error: {TraceId}", traceId);

        await context.Response.WriteAsync(JsonSerializer.Serialize(errorResponse, JsonOptions));
    }
}
