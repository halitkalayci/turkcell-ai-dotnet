using System;
using TurkcellAI.Core.Application.Abstractions.Messaging;

namespace TurkcellAI.Contracts.Orders.V1
{
    /// <summary>
    /// Integration event raised when an order is created.
    /// </summary>
    public sealed record OrderCreated(
        Guid Id,
        DateTime OccurredAtUtc,
        string Source,
        string Version,
        Guid OrderId,
        Guid CustomerId,
        decimal TotalAmount,
        string Currency,
        DateTime CreatedAtUtc
    ) : IIntegrationEvent;
}
