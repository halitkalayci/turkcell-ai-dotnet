using MediatR;
using OrderService.Domain.Enums;

namespace OrderService.Application.Commands.UpdateOrderStatus;

/// <summary>
/// Command to update an order's status.
/// Encapsulates the intent to change order status with business rules enforcement.
/// </summary>
public class UpdateOrderStatusCommand : IRequest<Unit>
{
    public Guid OrderId { get; set; }
    public OrderStatus NewStatus { get; set; }
}
