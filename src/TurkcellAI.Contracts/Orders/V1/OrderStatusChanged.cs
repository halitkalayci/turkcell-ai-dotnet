using System;
using TurkcellAI.Core.Application.Abstractions.Messaging;

namespace TurkcellAI.Contracts.Orders.V1
{
    /// <summary>
    /// Integration event raised when an order's status changes.
    /// </summary>
    public sealed record OrderStatusChanged(
        Guid Id,
        DateTime OccurredAtUtc,
        string Source,
        string Version,
        Guid OrderId,
        OrderStatus OldStatus,
        OrderStatus NewStatus,
        DateTime ChangedAtUtc,
        string? Reason
    ) : IIntegrationEvent;

    /// <summary>
    /// Public contract enum for order status to avoid leaking service-internal enums.
    /// </summary>
    public enum OrderStatus
    {
        Unknown = 0,
        Pending = 1,
        Confirmed = 2,
        Shipped = 3,
        Delivered = 4,
        Cancelled = 5,
        Failed = 6
    }
}
