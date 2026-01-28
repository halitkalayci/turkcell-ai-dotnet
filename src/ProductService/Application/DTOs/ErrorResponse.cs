namespace ProductService.Application.DTOs;

public class ErrorResponse
{
    public required string TraceId { get; set; }
    public required string Code { get; set; }
    public required string Message { get; set; }
    public string? Details { get; set; }
    public List<ValidationError>? Errors { get; set; }
}

public class ValidationError
{
    public required string Field { get; set; }
    public required string Message { get; set; }
}
