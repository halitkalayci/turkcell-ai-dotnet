using AutoMapper;
using MediatR;
using OrderService.Application.DTOs;
using OrderService.Application.Ports;
using OrderService.Domain.Entities;
using OrderService.Domain.ValueObjects;
using MassTransit;
using TurkcellAI.Contracts.Orders.V1;

namespace OrderService.Application.Commands.CreateOrder;

/// <summary>
/// Handler for CreateOrderCommand.
/// Creates a new order using the rich domain model.
/// </summary>
public class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand, OrderResponse>
{
    private readonly IOrderRepository _orderRepository;
    private readonly IMapper _mapper;
    private readonly IPublishEndpoint _publishEndpoint;

    public CreateOrderCommandHandler(IOrderRepository orderRepository, IMapper mapper, IPublishEndpoint publishEndpoint)
    {
        _orderRepository = orderRepository;
        _mapper = mapper;
        _publishEndpoint = publishEndpoint;
    }

    public async Task<OrderResponse> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
    {
        // Create order using domain model factory method
        var order = Order.Create(
            Guid.NewGuid(),
            CustomerId.Create(request.CustomerId),
            DateTime.UtcNow
        );

        // Add items using domain aggregate method
        foreach (var item in request.Items)
        {
            order.AddItem(
                item.ProductId,
                item.ProductName,
                item.Quantity,
                Money.Create(item.UnitPrice)
            );
        }

        // Persist the aggregate
        var createdOrder = await _orderRepository.CreateAsync(order);

        // Publish integration event (captured by Outbox if configured)
        var evt = new OrderCreated(
            Id: Guid.NewGuid(),
            OccurredAtUtc: DateTime.UtcNow,
            Source: "order-service",
            Version: "1.0",
            OrderId: createdOrder.Id,
            CustomerId: createdOrder.CustomerId.Value,
            TotalAmount: createdOrder.TotalAmount.Amount,
            Currency: createdOrder.TotalAmount.Currency,
            CreatedAtUtc: createdOrder.OrderDate
        );
        await _publishEndpoint.Publish(evt, cancellationToken);

        // Map to response DTO
        return _mapper.Map<OrderResponse>(createdOrder);
    }
}
