using MediatR;
using OrderService.Application.Ports;

namespace OrderService.Application.Commands.UpdateOrderStatus;

/// <summary>
/// Handler for UpdateOrderStatusCommand.
/// Updates order status using domain business rules.
/// </summary>
public class UpdateOrderStatusCommandHandler : IRequestHandler<UpdateOrderStatusCommand, Unit>
{
    private readonly IOrderRepository _orderRepository;

    public UpdateOrderStatusCommandHandler(IOrderRepository orderRepository)
    {
        _orderRepository = orderRepository;
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

        return Unit.Value;
    }
}
