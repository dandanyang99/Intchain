using Intchain.OrderService.Data;
using Intchain.OrderService.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Intchain.OrderService.Services;

/// <summary>
/// 订单状态转换历史记录服务实现
/// </summary>
public class OrderStatusHistoryService : IOrderStatusHistoryService
{
    private readonly OrderDbContext _context;
    private readonly ILogger<OrderStatusHistoryService> _logger;

    public OrderStatusHistoryService(
        OrderDbContext context,
        ILogger<OrderStatusHistoryService> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <summary>
    /// 记录状态转换
    /// </summary>
    public async Task RecordStatusTransitionAsync(
        string orderType,
        int orderId,
        string fromStatus,
        string toStatus,
        int? operatorId = null,
        string? operatorName = null,
        string? reason = null)
    {
        var history = new OrderStatusHistory
        {
            OrderType = orderType,
            OrderId = orderId,
            FromStatus = fromStatus,
            ToStatus = toStatus,
            OperatorId = operatorId,
            OperatorName = operatorName,
            Reason = reason,
            CreatedAt = DateTime.UtcNow
        };

        _context.OrderStatusHistories.Add(history);
        await _context.SaveChangesAsync();

        // 记录日志
        _logger.LogInformation(
            "订单状态转换: OrderType={OrderType}, OrderId={OrderId}, {FromStatus} -> {ToStatus}, Operator={OperatorName}",
            orderType, orderId, fromStatus, toStatus, operatorName ?? "System");
    }

    /// <summary>
    /// 获取订单的状态转换历史
    /// </summary>
    public async Task<List<OrderStatusHistory>> GetOrderHistoryAsync(string orderType, int orderId)
    {
        return await _context.OrderStatusHistories
            .Where(h => h.OrderType == orderType && h.OrderId == orderId)
            .OrderBy(h => h.CreatedAt)
            .ToListAsync();
    }

    /// <summary>
    /// 获取最近的状态转换记录
    /// </summary>
    public async Task<OrderStatusHistory?> GetLatestTransitionAsync(string orderType, int orderId)
    {
        return await _context.OrderStatusHistories
            .Where(h => h.OrderType == orderType && h.OrderId == orderId)
            .OrderByDescending(h => h.CreatedAt)
            .FirstOrDefaultAsync();
    }
}
