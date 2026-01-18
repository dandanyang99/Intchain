using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Intchain.OrderService.Models;

/// <summary>
/// 印刷订单实体（发给印刷厂）
/// </summary>
[Table("printing_orders")]
public class PrintingOrder
{
    /// <summary>
    /// 印刷订单ID
    /// </summary>
    [Key]
    [Column("id")]
    public int Id { get; set; }

    /// <summary>
    /// 印刷订单号
    /// </summary>
    [Required]
    [MaxLength(50)]
    [Column("order_number")]
    public string OrderNumber { get; set; } = string.Empty;

    /// <summary>
    /// 关联的申请订单ID（可选，发布产品时创建的印刷订单可能没有关联申请订单）
    /// </summary>
    [Column("application_order_id")]
    public int? ApplicationOrderId { get; set; }

    /// <summary>
    /// 印刷厂ID
    /// </summary>
    [Required]
    [Column("printing_factory_id")]
    public int PrintingFactoryId { get; set; }

    /// <summary>
    /// 彩票产品ID
    /// </summary>
    [Required]
    [Column("product_id")]
    public int ProductId { get; set; }

    /// <summary>
    /// 印刷数量
    /// </summary>
    [Required]
    [Column("quantity")]
    public int Quantity { get; set; }

    /// <summary>
    /// 订单状态: Pending, InProduction, WaitingShipment, Shipped, Completed
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
