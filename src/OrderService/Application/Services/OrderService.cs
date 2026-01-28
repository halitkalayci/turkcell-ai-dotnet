using OrderService.Application.DTOs;
using OrderService.Application.Ports;
using OrderService.Domain.Entities;
using OrderService.Domain.Enums;

namespace OrderService.Application.Services;

public class OrderService : IOrderService
{
    private readonly IOrderRepository _repository;

    public OrderService(IOrderRepository repository)
    {
        _repository = repository;
    }

    public async Task<OrderResponse> CreateOrderAsync(CreateOrderRequest request)
    {
        // Mock product data - in real scenario, this would come from Product Service
        var mockProducts = new Dictionary<Guid, (string Name, decimal Price)>();
        
        var order = new Order
        {
            OrderId = Guid.NewGuid(),
            CustomerId = request.CustomerId,
            OrderDate = DateTime.UtcNow,
            Status = OrderStatus.Pending,
            Items = new List<OrderItem>()
        };

        decimal totalAmount = 0;

        foreach (var itemRequest in request.Items)
        {
            // Mock product lookup
            var mockPrice = 100m;
            var mockName = $"Product-{itemRequest.ProductId}";
            
            var orderItem = new OrderItem
            {
                OrderItemId = Guid.NewGuid(),
                ProductId = itemRequest.ProductId,
                ProductName = mockName,
                Quantity = itemRequest.Quantity,
                UnitPrice = mockPrice,
                TotalPrice = mockPrice * itemRequest.Quantity,
                OrderId = order.OrderId
            };

            order.Items.Add(orderItem);
            totalAmount += orderItem.TotalPrice;
        }

        order.TotalAmount = totalAmount;

        var createdOrder = await _repository.CreateAsync(order);
        return MapToResponse(createdOrder);
    }

    public async Task<OrderResponse?> GetOrderByIdAsync(Guid orderId)
    {
        var order = await _repository.GetByIdAsync(orderId);
        return order == null ? null : MapToResponse(order);
    }

    public async Task<OrderListResponse> GetOrdersAsync(
        int pageNumber, 
        int pageSize, 
        OrderStatus? status, 
        Guid? customerId)
    {
        var (items, totalCount) = await _repository.GetAllAsync(pageNumber, pageSize, status, customerId);
        
        var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

        return new OrderListResponse
        {
            Items = items.Select(MapToResponse).ToList(),
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalCount = totalCount,
            TotalPages = totalPages
        };
    }

    public async Task<OrderResponse?> UpdateOrderStatusAsync(Guid orderId, UpdateOrderStatusRequest request)
    {
        var order = await _repository.GetByIdAsync(orderId);
        
        if (order == null)
            return null;

        if (Enum.TryParse<OrderStatus>(request.Status, true, out var newStatus))
        {
            order.Status = newStatus;
            var updatedOrder = await _repository.UpdateAsync(order);
            return MapToResponse(updatedOrder);
        }

        return null;
    }

    private static OrderResponse MapToResponse(Order order)
    {
        return new OrderResponse
        {
            OrderId = order.OrderId,
            CustomerId = order.CustomerId,
            OrderDate = order.OrderDate,
            Status = order.Status.ToString(),
            TotalAmount = order.TotalAmount,
            Items = order.Items.Select(i => new OrderItemResponse
            {
                OrderItemId = i.OrderItemId,
                ProductId = i.ProductId,
                ProductName = i.ProductName,
                Quantity = i.Quantity,
                UnitPrice = i.UnitPrice,
                TotalPrice = i.TotalPrice
            }).ToList()
        };
    }
}
