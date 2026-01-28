using AutoMapper;
using MediatR;
using OrderService.Application.DTOs;
using OrderService.Application.Ports;

namespace OrderService.Application.Queries.GetOrders;

/// <summary>
/// Handler for GetOrdersQuery.
/// Retrieves paginated orders with optional filtering.
/// </summary>
public class GetOrdersQueryHandler : IRequestHandler<GetOrdersQuery, OrderListResponse>
{
    private readonly IOrderRepository _orderRepository;
    private readonly IMapper _mapper;

    public GetOrdersQueryHandler(IOrderRepository orderRepository, IMapper mapper)
    {
        _orderRepository = orderRepository;
        _mapper = mapper;
    }

    public async Task<OrderListResponse> Handle(GetOrdersQuery request, CancellationToken cancellationToken)
    {
        var (orders, totalCount) = await _orderRepository.GetAllAsync(
            request.PageNumber,
            request.PageSize,
            request.Status,
            request.CustomerId
        );

        var orderResponses = _mapper.Map<List<OrderResponse>>(orders);

        return new OrderListResponse
        {
            Items = orderResponses,
            TotalCount = totalCount,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize
        };
    }
}
