using System.Diagnostics;
using System.Text.Json;
using FluentValidation;
using OrderService.Application.DTOs;

namespace OrderService.Middleware;

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
        catch (ValidationException ex)
        {
            await HandleValidationExceptionAsync(context, ex);
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
            Code = "VALIDATION_ERROR",
            Message = "One or more validation errors occurred",
            Errors = exception.Errors.Select(e => new ValidationError
            {
                Field = e.PropertyName,
                Message = e.ErrorMessage
            }).ToList()
        };

        _logger.LogWarning("Validation error: {TraceId}", traceId);

        await context.Response.WriteAsync(JsonSerializer.Serialize(errorResponse));
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = StatusCodes.Status500InternalServerError;

        var traceId = Activity.Current?.Id ?? context.TraceIdentifier;

        var errorResponse = new ErrorResponse
        {
            TraceId = traceId,
            Code = "INTERNAL_ERROR",
            Message = "An unexpected error occurred",
            Details = exception.Message
        };

        _logger.LogError(exception, "Internal error: {TraceId}", traceId);

        await context.Response.WriteAsync(JsonSerializer.Serialize(errorResponse));
    }
}
