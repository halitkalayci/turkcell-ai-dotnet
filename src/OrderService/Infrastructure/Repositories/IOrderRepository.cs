using OrderService.Domain.Entities;
using OrderService.Domain.Enums;

namespace OrderService.Infrastructure.Repositories;

public interface IOrderRepository
{
    Task<Order?> GetByIdAsync(Guid orderId);
    Task<(List<Order> Items, int TotalCount)> GetAllAsync(int pageNumber, int pageSize, OrderStatus? status, Guid? customerId);
    Task<Order> CreateAsync(Order order);
    Task<Order> UpdateAsync(Order order);
}
