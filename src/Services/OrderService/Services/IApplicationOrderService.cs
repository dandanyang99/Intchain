using Intchain.OrderService.DTOs;

namespace Intchain.OrderService.Services;

/// <summary>
/// 申请订单服务接口
/// </summary>
public interface IApplicationOrderService
{
    /// <summary>
    /// 创建申请订单
    /// </summary>
    Task<ApplicationOrderResponse?> CreateApplicationOrderAsync(CreateApplicationOrderRequest request);

    /// <summary>
    /// 更新申请订单
    /// </summary>
    Task<ApplicationOrderResponse?> UpdateApplicationOrderAsync(int id, UpdateApplicationOrderRequest request);

    /// <summary>
    /// 删除申请订单
    /// </summary>
    Task<bool> DeleteApplicationOrderAsync(int id);

    /// <summary>
    /// 获取申请订单
    /// </summary>
    Task<ApplicationOrderResponse?> GetApplicationOrderAsync(int id);

    /// <summary>
    /// 根据订单号获取申请订单
    /// </summary>
    Task<ApplicationOrderResponse?> GetApplicationOrderByNumberAsync(string orderNumber);

    /// <summary>
    /// 获取所有申请订单
    /// </summary>
    Task<List<ApplicationOrderResponse>> GetAllApplicationOrdersAsync();

    /// <summary>
    /// 根据销售网点ID获取申请订单
    /// </summary>
    Task<List<ApplicationOrderResponse>> GetApplicationOrdersBySalesOutletAsync(int salesOutletId);

    /// <summary>
    /// 根据彩票中心ID获取申请订单
    /// </summary>
    Task<List<ApplicationOrderResponse>> GetApplicationOrdersByLotteryCenterAsync(int lotteryCenterId);

    /// <summary>
    /// 根据状态获取申请订单
    /// </summary>
    Task<List<ApplicationOrderResponse>> GetApplicationOrdersByStatusAsync(string status);

    /// <summary>
    /// 审批通过申请订单
    /// </summary>
    Task<OrderOperationResponse> ApproveApplicationOrderAsync(int id, ApproveApplicationOrderRequest request);

    /// <summary>
    /// 拒绝申请订单
    /// </summary>
    Task<OrderOperationResponse> RejectApplicationOrderAsync(int id, RejectApplicationOrderRequest request);

    /// <summary>
    /// 更新订单状态为待发货
    /// </summary>
    Task<OrderOperationResponse> UpdateToWaitingShipmentAsync(int id);

    /// <summary>
    /// 更新订单状态为已发货
    /// </summary>
    Task<OrderOperationResponse> UpdateToShippedAsync(int id);

    /// <summary>
    /// 更新订单状态为运输中
    /// </summary>
    Task<OrderOperationResponse> UpdateToInTransitAsync(int id);

    /// <summary>
    /// 完成订单
    /// </summary>
    Task<OrderOperationResponse> CompleteApplicationOrderAsync(int id);
}
