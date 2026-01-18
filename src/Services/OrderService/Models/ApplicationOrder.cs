using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Intchain.OrderService.Models;

/// <summary>
/// 申请订单实体（销售网点申请）
/// </summary>
[Table("application_orders")]
public class ApplicationOrder
{
    /// <summary>
    /// 订单ID
    /// </summary>
    [Key]
    [Column("id")]
    public int Id { get; set; }

    /// <summary>
    /// 订单号
    /// </summary>
    [Required]
    [MaxLength(50)]
    [Column("order_number")]
    public string OrderNumber { get; set; } = string.Empty;

    /// <summary>
    /// 销售网点ID
    /// </summary>
    [Required]
    [Column("sales_outlet_id")]
    public int SalesOutletId { get; set; }

    /// <summary>
    /// 彩票中心ID
    /// </summary>
    [Required]
    [Column("lottery_center_id")]
    public int LotteryCenterId { get; set; }

    /// <summary>
    /// 彩票产品ID
    /// </summary>
    [Required]
    [Column("product_id")]
    public int ProductId { get; set; }

    /// <summary>
    /// 申请数量
    /// </summary>
    [Required]
    [Column("quantity")]
    public int Quantity { get; set; }

    /// <summary>
    /// 订单状态: Pending, Approved, Rejected, WaitingShipment, Shipped, InTransit, Completed
    /// </summary>
    [Required]
    [MaxLength(20)]
    [Column("status")]
    public string Status { get; set; } = "Pending";

    /// <summary>
    /// 备注
    /// </summary>
    [MaxLength(500)]
    [Column("remarks")]
    public string? Remarks { get; set; }

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
