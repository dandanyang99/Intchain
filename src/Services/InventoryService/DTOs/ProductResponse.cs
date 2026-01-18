namespace Intchain.InventoryService.DTOs;

/// <summary>
/// 产品响应
/// </summary>
public class ProductResponse
{
    /// <summary>
    /// 产品ID
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// 产品名称
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 单价
    /// </summary>
    public decimal UnitPrice { get; set; }

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
    /// 创建时间
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// 更新时间
    /// </summary>
    public DateTime UpdatedAt { get; set; }
}
