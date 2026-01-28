namespace OrderService.Domain.Enums;

/// <summary>
/// Standardized error codes used across the application.
/// Provides consistent error identification for API consumers.
/// </summary>
public enum ErrorCode
{
    /// <summary>
    /// General validation error for request data.
    /// </summary>
    VALIDATION_ERROR,

    /// <summary>
    /// Invalid parameter value provided.
    /// </summary>
    INVALID_PARAMETER,

    /// <summary>
    /// Requested order was not found.
    /// </summary>
    ORDER_NOT_FOUND,

    /// <summary>
    /// Internal server error occurred.
    /// </summary>
    INTERNAL_ERROR,

    /// <summary>
    /// Invalid status transition attempted.
    /// </summary>
    INVALID_STATUS_TRANSITION,

    /// <summary>
    /// Resource conflict (e.g., duplicate creation).
    /// </summary>
    CONFLICT,

    /// <summary>
    /// Unauthorized access attempted.
    /// </summary>
    UNAUTHORIZED,

    /// <summary>
    /// Forbidden action attempted.
    /// </summary>
    FORBIDDEN
}
