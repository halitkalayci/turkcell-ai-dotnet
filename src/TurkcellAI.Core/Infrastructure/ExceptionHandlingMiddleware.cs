using System.Diagnostics;
using System.Net;
using System.Text.Json;
using FluentValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using TurkcellAI.Core.Application.DTOs;
using TurkcellAI.Core.Application.Enums;

namespace TurkcellAI.Core.Infrastructure;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;

    public ExceptionHandlingMiddleware(RequestDelegate next)
    {
        _next = next;
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

    private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var traceId = Activity.Current?.Id ?? context.TraceIdentifier;

        var response = new ErrorResponse
        {
            TraceId = traceId,
            Code = ErrorCode.INTERNAL_ERROR,
            Message = "An unexpected error occurred.",
            Details = exception.Message
        };

        var statusCode = HttpStatusCode.InternalServerError;

        switch (exception)
        {
            case ValidationException validationEx:
                response.Code = ErrorCode.VALIDATION_ERROR;
                response.Message = "Request validation failed.";
                response.Errors = validationEx.Errors
                    .Select(e => new ValidationError
                    {
                        Field = e.PropertyName ?? string.Empty,
                        Message = e.ErrorMessage
                    })
                    .ToList();
                statusCode = HttpStatusCode.BadRequest;
                break;

            case ArgumentException:
                response.Code = ErrorCode.INVALID_PARAMETER;
                response.Message = "Invalid parameter.";
                statusCode = HttpStatusCode.BadRequest;
                break;

            case KeyNotFoundException:
                response.Code = ErrorCode.NOT_FOUND;
                response.Message = "Resource not found.";
                statusCode = HttpStatusCode.NotFound;
                break;

            case UnauthorizedAccessException:
                response.Code = ErrorCode.UNAUTHORIZED;
                response.Message = "Unauthorized.";
                statusCode = HttpStatusCode.Unauthorized;
                break;

            // Add more mappings as needed (Forbidden, Conflict, etc.)
        }

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)statusCode;

        var json = JsonSerializer.Serialize(response, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = false
        });

        await context.Response.WriteAsync(json);
    }
}

public static class ExceptionHandlingMiddlewareExtensions
{
    public static IApplicationBuilder UseCoreExceptionHandling(this IApplicationBuilder app)
    {
        return app.UseMiddleware<TurkcellAI.Core.Infrastructure.ExceptionHandlingMiddleware>();
    }
}
