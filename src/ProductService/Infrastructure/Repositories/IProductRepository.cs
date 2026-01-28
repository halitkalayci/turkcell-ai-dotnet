using ProductService.Domain.Entities;

namespace ProductService.Infrastructure.Repositories;

public interface IProductRepository
{
    Task<Product?> GetByIdAsync(Guid id);
    Task<(List<Product> Products, int TotalCount)> GetAllAsync(
        int pageNumber, 
        int pageSize, 
        string? category, 
        decimal? minPrice, 
        decimal? maxPrice);
    Task AddAsync(Product product);
    void Update(Product product);
    void Delete(Product product);
    Task<int> SaveChangesAsync();
}
