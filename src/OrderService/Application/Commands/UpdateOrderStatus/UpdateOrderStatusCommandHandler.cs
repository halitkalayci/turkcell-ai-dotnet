using MediatR;
using OrderService.Application.Ports;
using MassTransit;
using TurkcellAI.Contracts.Orders.V1;
using DomainStatus = OrderService.Domain.Enums.OrderStatus;

namespace OrderService.Application.Commands.UpdateOrderStatus;

/// <summary>
/// Handler for UpdateOrderStatusCommand.
/// Updates order status using domain business rules.
/// </summary>
public class UpdateOrderStatusCommandHandler : IRequestHandler<UpdateOrderStatusCommand, Unit>
{
    private readonly IOrderRepository _orderRepository;
    private readonly IPublishEndpoint _publishEndpoint;

    public UpdateOrderStatusCommandHandler(IOrderRepository orderRepository, IPublishEndpoint publishEndpoint)
    {
        _orderRepository = orderRepository;
        _publishEndpoint = publishEndpoint;
    }

    public async Task<Unit> Handle(UpdateOrderStatusCommand request, CancellationToken cancellationToken)
    {
        // Retrieve the aggregate
        var order = await _orderRepository.GetByIdAsync(request.OrderId);
        
        if (order == null)
        {
            throw new KeyNotFoundException($"Order with ID {request.OrderId} not found");
        }

        // Use domain method to update status (enforces business rules)
        var oldStatus = order.Status;
        order.UpdateStatus(request.NewStatus);

        // Persist changes
        await _orderRepository.UpdateAsync(order);

        // Publish integration event (captured by Outbox if configured)
        var evt = new OrderStatusChanged(
            Id: Guid.NewGuid(),
            OccurredAtUtc: DateTime.UtcNow,
            Source: "order-service",
            Version: "1.0",
            OrderId: order.Id,
            OldStatus: MapToContractStatus(oldStatus),
            NewStatus: MapToContractStatus(order.Status),
            ChangedAtUtc: DateTime.UtcNow,
            Reason: null
        );
        await _publishEndpoint.Publish(evt, cancellationToken);

        return Unit.Value;
    }

    private static TurkcellAI.Contracts.Orders.V1.OrderStatus MapToContractStatus(DomainStatus status)
        => status switch
        {
            DomainStatus.Pending => TurkcellAI.Contracts.Orders.V1.OrderStatus.Pending,
            DomainStatus.Confirmed => TurkcellAI.Contracts.Orders.V1.OrderStatus.Confirmed,
            DomainStatus.Shipped => TurkcellAI.Contracts.Orders.V1.OrderStatus.Shipped,
            DomainStatus.Delivered => TurkcellAI.Contracts.Orders.V1.OrderStatus.Delivered,
            DomainStatus.Cancelled => TurkcellAI.Contracts.Orders.V1.OrderStatus.Cancelled,
            _ => TurkcellAI.Contracts.Orders.V1.OrderStatus.Unknown
        };
}
