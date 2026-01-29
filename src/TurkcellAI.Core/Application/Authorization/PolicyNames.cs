namespace TurkcellAI.Core.Application.Authorization;

public static class PolicyNames
{
    // Orders
    public const string OrdersCreate = "orders:create";
    public const string OrdersRead = "orders:read";
    public const string OrdersUpdateStatus = "orders:update_status";

    // Products
    public const string ProductsCreate = "products:create";
    public const string ProductsRead = "products:read";
    public const string ProductsUpdate = "products:update";
    public const string ProductsDelete = "products:delete";
}
