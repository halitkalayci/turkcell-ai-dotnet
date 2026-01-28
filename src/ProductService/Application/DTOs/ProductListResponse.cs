namespace ProductService.Application.DTOs;

public class ProductListResponse
{
    public required List<ProductResponse> Items { get; set; }
    public required int TotalCount { get; set; }
    public required int PageNumber { get; set; }
    public required int PageSize { get; set; }
    public required int TotalPages { get; set; }
}
