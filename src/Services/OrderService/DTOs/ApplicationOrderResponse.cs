namespace Intchain.OrderService.DTOs;

/// <summary>
/// 申请订单响应
/// </summary>
public class ApplicationOrderResponse
{
    public int Id { get; set; }
    public string OrderNumber { get; set; } = string.Empty;
    public int SalesOutletId { get; set; }
    public int LotteryCenterId { get; set; }
    public int ProductId { get; set; }
    public int Quantity { get; set; }
    public string Status { get; set; } = string.Empty;
    public string? Remarks { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
