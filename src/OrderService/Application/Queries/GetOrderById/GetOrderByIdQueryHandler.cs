using AutoMapper;
using MediatR;
using OrderService.Application.DTOs;
using OrderService.Application.Ports;

namespace OrderService.Application.Queries.GetOrderById;

/// <summary>
/// Handler for GetOrderByIdQuery.
/// Retrieves a single order and maps it to response DTO.
/// </summary>
public class GetOrderByIdQueryHandler : IRequestHandler<GetOrderByIdQuery, OrderResponse?>
{
    private readonly IOrderRepository _orderRepository;
    private readonly IMapper _mapper;

    public GetOrderByIdQueryHandler(IOrderRepository orderRepository, IMapper mapper)
    {
        _orderRepository = orderRepository;
        _mapper = mapper;
    }

    public async Task<OrderResponse?> Handle(GetOrderByIdQuery request, CancellationToken cancellationToken)
    {
        var order = await _orderRepository.GetByIdAsync(request.OrderId);

        if (order == null)
        {
            return null;
        }

        return _mapper.Map<OrderResponse>(order);
    }
}
