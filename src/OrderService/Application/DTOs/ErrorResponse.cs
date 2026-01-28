using OrderService.Domain.Enums;

namespace OrderService.Application.DTOs;

/// <summary>
/// Represents a single field-level validation error.
/// </summary>
public class ValidationError
{
    /// <summary>
    /// Name of the field that failed validation.
    /// </summary>
    public string Field { get; set; } = string.Empty;

    /// <summary>
    /// Validation error message for this field.
    /// </summary>
    public string Message { get; set; } = string.Empty;
}

/// <summary>
/// Standardized error response DTO.
/// Aligned with AGENTS.md Section 2.2 error model standard.
/// </summary>
public class ErrorResponse
{
    /// <summary>
    /// Unique identifier for tracing this specific request.
    /// </summary>
    public string TraceId { get; set; } = string.Empty;

    /// <summary>
    /// Standardized error code for programmatic error handling.
    /// </summary>
    public ErrorCode Code { get; set; }

    /// <summary>
    /// Human-readable error message.
    /// </summary>
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// Optional additional details about the error.
    /// </summary>
    public string? Details { get; set; }

    /// <summary>
    /// Optional list of field-level validation errors.
    /// </summary>
    public List<ValidationError>? Errors { get; set; }
}
