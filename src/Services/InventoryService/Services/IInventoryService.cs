using Intchain.InventoryService.DTOs;

namespace Intchain.InventoryService.Services;

/// <summary>
/// 库存服务接口
/// </summary>
public interface IInventoryService
{
    /// <summary>
    /// 获取产品
    /// </summary>
    Task<ProductResponse?> GetProductAsync(int productId);

    /// <summary>
    /// 根据彩票中心ID获取产品
    /// </summary>
    Task<List<ProductResponse>> GetProductsByLotteryCenterAsync(int lotteryCenterId);

    /// <summary>
    /// 预留库存
    /// </summary>
    Task<InventoryOperationResponse> ReserveInventoryAsync(int productId, int quantity, string orderId);

    /// <summary>
    /// 释放预留库存
    /// </summary>
    Task<InventoryOperationResponse> ReleaseReservedInventoryAsync(int productId, int quantity, string orderId);

    /// <summary>
    /// 确认库存扣减
    /// </summary>
    Task<InventoryOperationResponse> ConfirmInventoryDeductionAsync(int productId, int quantity, string orderId);

    /// <summary>
    /// 获取库存统计
    /// </summary>
    Task<InventoryStatsResponse> GetInventoryStatsAsync(int productId);
}
