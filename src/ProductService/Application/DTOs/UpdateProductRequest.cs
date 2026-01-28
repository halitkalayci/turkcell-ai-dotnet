namespace ProductService.Application.DTOs;

public class UpdateProductRequest
{
    public required string Name { get; set; }
    public string? Description { get; set; }
    public required decimal Price { get; set; }
    public required int StockQuantity { get; set; }
    public string? Category { get; set; }
}
