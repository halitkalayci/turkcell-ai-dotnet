namespace OrderService.Application.DTOs;

public class OrderListResponse
{
    public List<OrderResponse> Items { get; set; } = new();
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public long TotalCount { get; set; }
    public int TotalPages { get; set; }
}
