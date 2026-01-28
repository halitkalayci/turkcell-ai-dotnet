namespace ProductService.Domain.Entities;

/// <summary>
/// Product aggregate root - represents a product in the catalog
/// </summary>
public class Product : BaseEntity
{
    public string Name { get; private set; }
    public string? Description { get; private set; }
    public decimal Price { get; private set; }
    public int StockQuantity { get; private set; }
    public string? Category { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    // EF Core constructor
    private Product()
    {
        Name = string.Empty;
    }

    /// <summary>
    /// Creates a new product - factory method enforcing domain rules
    /// </summary>
    public Product(string name, decimal price, int stockQuantity, string? description = null, string? category = null)
    {
        ValidateName(name);
        ValidatePrice(price);
        ValidateStockQuantity(stockQuantity);

        Name = name;
        Price = price;
        StockQuantity = stockQuantity;
        Description = description;
        Category = category;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Updates product information
    /// </summary>
    public void Update(string name, decimal price, int stockQuantity, string? description = null, string? category = null)
    {
        ValidateName(name);
        ValidatePrice(price);
        ValidateStockQuantity(stockQuantity);

        Name = name;
        Price = price;
        StockQuantity = stockQuantity;
        Description = description;
        Category = category;
        UpdatedAt = DateTime.UtcNow;
    }

    private static void ValidateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Product name cannot be empty", nameof(name));
        
        if (name.Length > 200)
            throw new ArgumentException("Product name cannot exceed 200 characters", nameof(name));
    }

    private static void ValidatePrice(decimal price)
    {
        if (price <= 0)
            throw new ArgumentException("Product price must be greater than 0", nameof(price));
    }

    private static void ValidateStockQuantity(int stockQuantity)
    {
        if (stockQuantity < 0)
            throw new ArgumentException("Stock quantity cannot be negative", nameof(stockQuantity));
    }
}
