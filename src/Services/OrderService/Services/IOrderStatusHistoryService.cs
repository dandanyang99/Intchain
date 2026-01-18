using Intchain.OrderService.Models;

namespace Intchain.OrderService.Services;

/// <summary>
/// 订单状态转换历史记录服务接口
/// </summary>
public interface IOrderStatusHistoryService
{
    /// <summary>
    /// 记录状态转换
    /// </summary>
    Task RecordStatusTransitionAsync(
        string orderType,
        int orderId,
        string fromStatus,
        string toStatus,
        int? operatorId = null,
        string? operatorName = null,
        string? reason = null);

    /// <summary>
    /// 获取订单的状态转换历史
    /// </summary>
    Task<List<OrderStatusHistory>> GetOrderHistoryAsync(string orderType, int orderId);

    /// <summary>
    /// 获取最近的状态转换记录
    /// </summary>
    Task<OrderStatusHistory?> GetLatestTransitionAsync(string orderType, int orderId);
}
