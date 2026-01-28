using System.ComponentModel.DataAnnotations;

namespace OrderService.Application.DTOs;

/// <summary>
/// Request DTO for creating an order item.
/// Aligned with OpenAPI contract specification.
/// </summary>
public class CreateOrderItemRequest
{
    /// <summary>
    /// Unique identifier of the product.
    /// </summary>
    [Required]
    public Guid ProductId { get; set; }

    /// <summary>
    /// Name of the product.
    /// Minimum length: 1, Maximum length: 200
    /// </summary>
    [Required]
    [StringLength(200, MinimumLength = 1)]
    public string ProductName { get; set; } = string.Empty;

    /// <summary>
    /// Quantity of the product to order.
    /// Must be at least 1.
    /// </summary>
    [Range(1, int.MaxValue)]
    public int Quantity { get; set; }

    /// <summary>
    /// Price per unit of the product.
    /// Must be greater than 0.
    /// </summary>
    [Range(0.01, double.MaxValue)]
    public decimal UnitPrice { get; set; }
}
