namespace Intchain.OrderService.DTOs;

/// <summary>
/// 订单操作响应
/// </summary>
public class OrderOperationResponse
{
    /// <summary>
    /// 操作是否成功
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// 消息
    /// </summary>
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// 订单号
    /// </summary>
    public string? OrderNumber { get; set; }

    /// <summary>
    /// 订单ID
    /// </summary>
    public int? OrderId { get; set; }

    /// <summary>
    /// 当前状态
    /// </summary>
    public string? CurrentStatus { get; set; }
}
