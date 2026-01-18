using Intchain.UserService.DTOs;

namespace Intchain.UserService.Services;

/// <summary>
/// 销售网点服务接口
/// </summary>
public interface ISalesOutletService
{
    /// <summary>
    /// 创建销售网点（同时创建默认用户）
    /// </summary>
    Task<SalesOutletResponse> CreateSalesOutletAsync(CreateSalesOutletRequest request);

    /// <summary>
    /// 批量创建销售网点（同时为每个网点创建默认用户）
    /// </summary>
    Task<List<SalesOutletResponse>> BatchCreateSalesOutletsAsync(BatchCreateSalesOutletsRequest request);

    /// <summary>
    /// 获取销售网点
    /// </summary>
    Task<SalesOutletResponse?> GetSalesOutletAsync(int id);

    /// <summary>
    /// 获取所有销售网点
    /// </summary>
    Task<List<SalesOutletResponse>> GetAllSalesOutletsAsync();

    /// <summary>
    /// 根据彩票中心ID获取销售网点
    /// </summary>
    Task<List<SalesOutletResponse>> GetSalesOutletsByLotteryCenterIdAsync(int lotteryCenterId);

    /// <summary>
    /// 更新销售网点
    /// </summary>
    Task<SalesOutletResponse> UpdateSalesOutletAsync(int id, UpdateSalesOutletRequest request);

    /// <summary>
    /// 删除销售网点
    /// </summary>
    Task<bool> DeleteSalesOutletAsync(int id);
}
