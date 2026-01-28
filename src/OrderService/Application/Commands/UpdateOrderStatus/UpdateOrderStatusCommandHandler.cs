using MediatR;
using OrderService.Application.Ports;
using MassTransit;
using TurkcellAI.Contracts.Orders.V1;

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
            OldStatus: OrderStatus.Unknown, // domain value not exposed; map if available
            NewStatus: Enum.TryParse<OrderStatus>(request.NewStatus, true, out var ns) ? ns : OrderStatus.Unknown,
            ChangedAtUtc: DateTime.UtcNow,
            Reason: null
        );
        await _publishEndpoint.Publish(evt, cancellationToken);

        return Unit.Value;
    }
}
