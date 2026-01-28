using MediatR;
using OrderService.Application.DTOs;

namespace OrderService.Application.Commands.CreateOrder;

/// <summary>
/// Command to create a new order.
/// Encapsulates the intent to create an order in the system.
/// </summary>
public class CreateOrderCommand : IRequest<OrderResponse>
{
    public Guid CustomerId { get; set; }
    public List<CreateOrderItemRequest> Items { get; set; } = new();
}
