using MediatR;
using OrderService.Application.DTOs;
using OrderService.Domain.Enums;

namespace OrderService.Application.Queries.GetOrders;

/// <summary>
/// Query to retrieve a paginated list of orders.
/// Supports filtering by status and customer ID.
/// </summary>
public class GetOrdersQuery : IRequest<OrderListResponse>
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public OrderStatus? Status { get; set; }
    public Guid? CustomerId { get; set; }
}
