using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Intchain.InventoryService.Models;

/// <summary>
/// 彩票产品实体
/// </summary>
[Table("lottery_products")]
public class LotteryProduct
{
    /// <summary>
    /// 产品ID
    /// </summary>
    [Key]
    [Column("id")]
    public int Id { get; set; }

    /// <summary>
    /// 产品名称
    /// </summary>
    [Required]
    [MaxLength(100)]
    [Column("name")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 产品描述
    /// </summary>
    [MaxLength(500)]
    [Column("description")]
    public string? Description { get; set; }

    /// <summary>
    /// 所属彩票中心ID
    /// </summary>
    [Required]
    [Column("lottery_center_id")]
    public int LotteryCenterId { get; set; }

    /// <summary>
    /// 单价
    /// </summary>
    [Required]
    [Column("unit_price", TypeName = "decimal(18,2)")]
    public decimal UnitPrice { get; set; }

    /// <summary>
    /// 总库存
    /// </summary>
    [Required]
    [Column("total_stock")]
    public int TotalStock { get; set; }

    /// <summary>
    /// 可用库存
    /// </summary>
    [Required]
    [Column("available_stock")]
    public int AvailableStock { get; set; }

    /// <summary>
    /// 预留库存（待审批）
    /// </summary>
    [Required]
    [Column("reserved_stock")]
    public int ReservedStock { get; set; }

    /// <summary>
    /// 是否激活
    /// </summary>
    [Column("is_active")]
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// 创建时间
    /// </summary>
    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// 更新时间
    /// </summary>
    [Column("updated_at")]
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
