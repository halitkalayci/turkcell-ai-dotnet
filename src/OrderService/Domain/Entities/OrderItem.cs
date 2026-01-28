using TurkcellAI.Core.Domain;
using OrderService.Domain.ValueObjects;

namespace OrderService.Domain.Entities;

/// <summary>
/// Order item entity representing a product within an order.
/// Encapsulates item-level business logic and calculations.
/// </summary>
public class OrderItem : Entity<Guid>
{
    public Guid ProductId { get; private set; }
    public string ProductName { get; private set; }
    public int Quantity { get; private set; }
    public Money UnitPrice { get; private set; }
    public Money TotalPrice { get; private set; }

    // Private constructor for entity creation
    private OrderItem(Guid id, Guid productId, string productName, int quantity, Money unitPrice) : base(id)
    {
        ProductId = productId;
        ProductName = productName;
        Quantity = quantity;
        UnitPrice = unitPrice;
        TotalPrice = unitPrice.Multiply(quantity);
    }

    // Required for EF Core
#pragma warning disable CS8618
    private OrderItem() : base() { }
#pragma warning restore CS8618

    /// <summary>
    /// Factory method to create a new OrderItem.
    /// Validates input and calculates total price.
    /// </summary>
    public static OrderItem Create(Guid orderItemId, Guid productId, string productName, int quantity, Money unitPrice)
    {
        if (orderItemId == Guid.Empty)
            throw new ArgumentException("Order item ID cannot be empty", nameof(orderItemId));

        if (productId == Guid.Empty)
            throw new ArgumentException("Product ID cannot be empty", nameof(productId));

        if (string.IsNullOrWhiteSpace(productName))
            throw new ArgumentException("Product name cannot be empty", nameof(productName));

        if (quantity <= 0)
            throw new ArgumentException("Quantity must be greater than zero", nameof(quantity));

        return new OrderItem(orderItemId, productId, productName, quantity, unitPrice);
    }

    /// <summary>
    /// Updates the quantity and recalculates total price.
    /// </summary>
    public void UpdateQuantity(int newQuantity)
    {
        if (newQuantity <= 0)
            throw new ArgumentException("Quantity must be greater than zero", nameof(newQuantity));

        Quantity = newQuantity;
        TotalPrice = UnitPrice.Multiply(newQuantity);
    }
}
