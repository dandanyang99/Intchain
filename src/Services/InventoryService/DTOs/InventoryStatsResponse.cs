namespace Intchain.InventoryService.DTOs;

/// <summary>
/// 库存统计响应
/// </summary>
public class InventoryStatsResponse
{
    /// <summary>
    /// 产品ID
    /// </summary>
    public int ProductId { get; set; }

    /// <summary>
    /// 总库存
    /// </summary>
    public int TotalStock { get; set; }

    /// <summary>
    /// 可用库存
    /// </summary>
    public int AvailableStock { get; set; }

    /// <summary>
    /// 预留库存
    /// </summary>
    public int ReservedStock { get; set; }

    /// <summary>
    /// 已售库存
    /// </summary>
    public int SoldStock { get; set; }
}
