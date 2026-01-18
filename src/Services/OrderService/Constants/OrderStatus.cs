namespace Intchain.OrderService.Constants;

/// <summary>
/// 订单状态常量
/// </summary>
public static class OrderStatus
{
    // 申请订单状态
    public const string ApplicationPending = "Pending";
    public const string ApplicationApproved = "Approved";
    public const string ApplicationRejected = "Rejected";
    public const string ApplicationWaitingShipment = "WaitingShipment";
    public const string ApplicationShipped = "Shipped";
    public const string ApplicationInTransit = "InTransit";
    public const string ApplicationCompleted = "Completed";

    // 印刷订单状态
    public const string PrintingPending = "Pending";
    public const string PrintingInProduction = "InProduction";
    public const string PrintingWaitingShipment = "WaitingShipment";
    public const string PrintingShipped = "Shipped";
    public const string PrintingCompleted = "Completed";
}
