using System.ComponentModel.DataAnnotations;

namespace OrderService.Application.DTOs;

/// <summary>
/// Request DTO for creating a new order.
/// Aligned with OpenAPI contract specification.
/// </summary>
public class CreateOrderRequest
{
    /// <summary>
    /// Unique identifier of the customer placing the order.
    /// </summary>
    [Required]
    public Guid CustomerId { get; set; }

    /// <summary>
    /// List of items to include in the order.
    /// Must contain at least one item.
    /// </summary>
    [Required]
    [MinLength(1)]
    public List<CreateOrderItemRequest> Items { get; set; } = new();
}
