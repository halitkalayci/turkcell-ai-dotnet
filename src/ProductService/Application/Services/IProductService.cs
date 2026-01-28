using ProductService.Application.DTOs;

namespace ProductService.Application.Services;

public interface IProductService
{
    Task<ProductResponse> CreateAsync(CreateProductRequest request);
    Task<ProductListResponse> GetAllAsync(int pageNumber, int pageSize, string? category, decimal? minPrice, decimal? maxPrice);
    Task<ProductResponse?> GetByIdAsync(Guid id);
    Task<ProductResponse?> UpdateAsync(Guid id, UpdateProductRequest request);
    Task<bool> DeleteAsync(Guid id);
}
