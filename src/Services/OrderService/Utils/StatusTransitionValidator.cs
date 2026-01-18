namespace Intchain.OrderService.Utils;

using Intchain.OrderService.Constants;

/// <summary>
/// 状态转换验证器
/// </summary>
public static class StatusTransitionValidator
{
    /// <summary>
    /// 验证申请订单状态转换是否合法
    /// </summary>
    public static bool IsValidApplicationOrderTransition(string currentStatus, string newStatus)
    {
        return (currentStatus, newStatus) switch
        {
            (OrderStatus.ApplicationPending, OrderStatus.ApplicationApproved) => true,
            (OrderStatus.ApplicationPending, OrderStatus.ApplicationRejected) => true,
            (OrderStatus.ApplicationApproved, OrderStatus.ApplicationWaitingShipment) => true,
            (OrderStatus.ApplicationWaitingShipment, OrderStatus.ApplicationShipped) => true,
            (OrderStatus.ApplicationShipped, OrderStatus.ApplicationInTransit) => true,
            (OrderStatus.ApplicationInTransit, OrderStatus.ApplicationCompleted) => true,
            _ => false
        };
    }

    /// <summary>
    /// 验证印刷订单状态转换是否合法
    /// </summary>
    public static bool IsValidPrintingOrderTransition(string currentStatus, string newStatus)
    {
        return (currentStatus, newStatus) switch
        {
            (OrderStatus.PrintingPending, OrderStatus.PrintingInProduction) => true,
            (OrderStatus.PrintingInProduction, OrderStatus.PrintingWaitingShipment) => true,
            (OrderStatus.PrintingWaitingShipment, OrderStatus.PrintingShipped) => true,
            (OrderStatus.PrintingShipped, OrderStatus.PrintingCompleted) => true,
            _ => false
        };
    }

    /// <summary>
    /// 获取状态转换错误消息
    /// </summary>
    public static string GetTransitionErrorMessage(string currentStatus, string newStatus)
    {
        return $"无法从状态 '{currentStatus}' 转换到 '{newStatus}'";
    }
}
