using OrderService.Application.DTOs;
using OrderService.Domain.Enums;

namespace OrderService.Application.Services;

public interface IOrderService
{
    Task<OrderResponse> CreateOrderAsync(CreateOrderRequest request);
    Task<OrderResponse?> GetOrderByIdAsync(Guid orderId);
    Task<OrderListResponse> GetOrdersAsync(int pageNumber, int pageSize, OrderStatus? status, Guid? customerId);
    Task<OrderResponse?> UpdateOrderStatusAsync(Guid orderId, UpdateOrderStatusRequest request);
}
