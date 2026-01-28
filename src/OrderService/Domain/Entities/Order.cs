using OrderService.Domain.Common;
using OrderService.Domain.Enums;
using OrderService.Domain.ValueObjects;

namespace OrderService.Domain.Entities;

/// <summary>
/// Order aggregate root.
/// Encapsulates order business logic and ensures consistency within the aggregate boundary.
/// </summary>
public class Order : AggregateRoot<Guid>
{
    private readonly List<OrderItem> _items = new();

    public CustomerId CustomerId { get; private set; }
    public DateTime OrderDate { get; private set; }
    public OrderStatus Status { get; private set; }
    public Money TotalAmount { get; private set; }
    
    public IReadOnlyCollection<OrderItem> Items => _items.AsReadOnly();

    // Private constructor for aggregate creation
    private Order(Guid id, CustomerId customerId, DateTime orderDate) : base(id)
    {
        CustomerId = customerId;
        OrderDate = orderDate;
        Status = OrderStatus.Pending;
        TotalAmount = Money.Zero();
    }

    // Required for EF Core
#pragma warning disable CS8618
    private Order() : base() { }
#pragma warning restore CS8618

    /// <summary>
    /// Factory method to create a new Order aggregate.
    /// </summary>
    public static Order Create(Guid orderId, CustomerId customerId, DateTime orderDate)
    {
        if (orderId == Guid.Empty)
            throw new ArgumentException("Order ID cannot be empty", nameof(orderId));

        if (orderDate == default)
            throw new ArgumentException("Order date cannot be default", nameof(orderDate));

        return new Order(orderId, customerId, orderDate);
    }

    /// <summary>
    /// Adds an item to the order.
    /// Recalculates total amount after adding.
    /// </summary>
    public void AddItem(Guid productId, string productName, int quantity, Money unitPrice)
    {
        if (Status != OrderStatus.Pending)
            throw new InvalidOperationException($"Cannot add items to order with status {Status}");

        var orderItem = OrderItem.Create(Guid.NewGuid(), productId, productName, quantity, unitPrice);
        _items.Add(orderItem);
        
        CalculateTotal();
    }

    /// <summary>
    /// Updates the order status.
    /// Enforces valid status transitions.
    /// </summary>
    public void UpdateStatus(OrderStatus newStatus)
    {
        if (!IsValidStatusTransition(Status, newStatus))
            throw new InvalidOperationException($"Cannot transition from {Status} to {newStatus}");

        Status = newStatus;
    }

    /// <summary>
    /// Recalculates the total amount based on all order items.
    /// </summary>
    private void CalculateTotal()
    {
        var total = Money.Zero();
        foreach (var item in _items)
        {
            total = total.Add(item.TotalPrice);
        }
        TotalAmount = total;
    }

    private static bool IsValidStatusTransition(OrderStatus currentStatus, OrderStatus newStatus)
    {
        return (currentStatus, newStatus) switch
        {
            (OrderStatus.Pending, OrderStatus.Confirmed) => true,
            (OrderStatus.Pending, OrderStatus.Cancelled) => true,
            (OrderStatus.Confirmed, OrderStatus.Shipped) => true,
            (OrderStatus.Confirmed, OrderStatus.Cancelled) => true,
            (OrderStatus.Shipped, OrderStatus.Delivered) => true,
            _ => false
        };
    }
}
