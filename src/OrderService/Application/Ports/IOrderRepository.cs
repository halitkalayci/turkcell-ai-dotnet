using OrderService.Domain.Entities;
using OrderService.Domain.Enums;

namespace OrderService.Application.Ports;

/// <summary>
/// Repository interface for Order aggregate.
/// Defines the contract for order persistence operations.
/// This is a port in the hexagonal architecture - implemented by Infrastructure layer.
/// </summary>
public interface IOrderRepository
{
    Task<Order?> GetByIdAsync(Guid orderId);
    Task<(List<Order> Items, int TotalCount)> GetAllAsync(int pageNumber, int pageSize, OrderStatus? status, Guid? customerId);
    Task<Order> CreateAsync(Order order);
    Task<Order> UpdateAsync(Order order);
}
