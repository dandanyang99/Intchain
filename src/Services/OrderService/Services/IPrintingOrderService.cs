using Intchain.OrderService.DTOs;

namespace Intchain.OrderService.Services;

/// <summary>
/// 印刷订单服务接口
/// </summary>
public interface IPrintingOrderService
{
    /// <summary>
    /// 创建印刷订单
    /// </summary>
    Task<PrintingOrderResponse?> CreatePrintingOrderAsync(CreatePrintingOrderRequest request);

    /// <summary>
    /// 获取印刷订单
    /// </summary>
    Task<PrintingOrderResponse?> GetPrintingOrderAsync(int id);

    /// <summary>
    /// 根据订单号获取印刷订单
    /// </summary>
    Task<PrintingOrderResponse?> GetPrintingOrderByNumberAsync(string orderNumber);

    /// <summary>
    /// 根据申请订单ID获取印刷订单
    /// </summary>
    Task<PrintingOrderResponse?> GetPrintingOrderByApplicationOrderIdAsync(int applicationOrderId);

    /// <summary>
    /// 获取所有印刷订单
    /// </summary>
    Task<List<PrintingOrderResponse>> GetAllPrintingOrdersAsync();

    /// <summary>
    /// 根据印刷厂ID获取印刷订单
    /// </summary>
    Task<List<PrintingOrderResponse>> GetPrintingOrdersByFactoryAsync(int printingFactoryId);

    /// <summary>
    /// 根据状态获取印刷订单
    /// </summary>
    Task<List<PrintingOrderResponse>> GetPrintingOrdersByStatusAsync(string status);

    /// <summary>
    /// 接单（更新状态为生产中）
    /// </summary>
    Task<OrderOperationResponse> AcceptPrintingOrderAsync(int id);

    /// <summary>
    /// 更新订单状态为待发货
    /// </summary>
    Task<OrderOperationResponse> UpdateToWaitingShipmentAsync(int id);

    /// <summary>
    /// 更新订单状态为已发货
    /// </summary>
    Task<OrderOperationResponse> UpdateToShippedAsync(int id);

    /// <summary>
    /// 完成印刷订单
    /// </summary>
    Task<OrderOperationResponse> CompletePrintingOrderAsync(int id);
}
