using MediatR;
using OrderService.Application.DTOs;

namespace OrderService.Application.Queries.GetOrderById;

/// <summary>
/// Query to retrieve a single order by its ID.
/// Read-only operation that returns order details.
/// </summary>
public class GetOrderByIdQuery : IRequest<OrderResponse?>
{
    public Guid OrderId { get; set; }

    public GetOrderByIdQuery(Guid orderId)
    {
        OrderId = orderId;
    }
}
