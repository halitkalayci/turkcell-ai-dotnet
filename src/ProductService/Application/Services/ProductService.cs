using ProductService.Application.DTOs;
using ProductService.Infrastructure.Repositories;

namespace ProductService.Application.Services;

public class ProductService : IProductService
{
    private readonly IProductRepository _repository;

    public ProductService(IProductRepository repository)
    {
        _repository = repository;
    }

    public async Task<ProductResponse> CreateAsync(CreateProductRequest request)
    {
        var product = new Domain.Entities.Product(
            name: request.Name,
            price: request.Price,
            stockQuantity: request.StockQuantity,
            description: request.Description,
            category: request.Category
        );

        await _repository.AddAsync(product);
        await _repository.SaveChangesAsync();

        return MapToResponse(product);
    }

    public async Task<ProductListResponse> GetAllAsync(
        int pageNumber, 
        int pageSize, 
        string? category, 
        decimal? minPrice, 
        decimal? maxPrice)
    {
        var (products, totalCount) = await _repository.GetAllAsync(
            pageNumber, 
            pageSize, 
            category, 
            minPrice, 
            maxPrice);

        var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

        return new ProductListResponse
        {
            Items = products.Select(MapToResponse).ToList(),
            TotalCount = totalCount,
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalPages = totalPages
        };
    }

    public async Task<ProductResponse?> GetByIdAsync(Guid id)
    {
        var product = await _repository.GetByIdAsync(id);
        return product == null ? null : MapToResponse(product);
    }

    public async Task<ProductResponse?> UpdateAsync(Guid id, UpdateProductRequest request)
    {
        var product = await _repository.GetByIdAsync(id);
        if (product == null)
            return null;

        product.Update(
            name: request.Name,
            price: request.Price,
            stockQuantity: request.StockQuantity,
            description: request.Description,
            category: request.Category
        );

        _repository.Update(product);
        await _repository.SaveChangesAsync();

        return MapToResponse(product);
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var product = await _repository.GetByIdAsync(id);
        if (product == null)
            return false;

        _repository.Delete(product);
        await _repository.SaveChangesAsync();

        return true;
    }

    private static ProductResponse MapToResponse(Domain.Entities.Product product)
    {
        return new ProductResponse
        {
            Id = product.Id,
            Name = product.Name,
            Description = product.Description,
            Price = product.Price,
            StockQuantity = product.StockQuantity,
            Category = product.Category,
            CreatedAt = product.CreatedAt,
            UpdatedAt = product.UpdatedAt
        };
    }
}
