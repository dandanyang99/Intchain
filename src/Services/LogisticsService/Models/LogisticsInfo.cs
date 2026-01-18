using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Intchain.LogisticsService.Models;

/// <summary>
/// 物流信息实体
/// </summary>
[Table("logistics_info")]
public class LogisticsInfo
{
    /// <summary>
    /// 物流信息ID
    /// </summary>
    [Key]
    [Column("id")]
    public int Id { get; set; }

    /// <summary>
    /// 印刷订单ID
    /// </summary>
    [Required]
    [Column("printing_order_id")]
    public int PrintingOrderId { get; set; }

    /// <summary>
    /// 物流公司
    /// </summary>
    [Required]
    [MaxLength(100)]
    [Column("logistics_company")]
    public string LogisticsCompany { get; set; } = string.Empty;

    /// <summary>
    /// 物流单号
    /// </summary>
    [Required]
    [MaxLength(100)]
    [Column("tracking_number")]
    public string TrackingNumber { get; set; } = string.Empty;

    /// <summary>
    /// 发货地址
    /// </summary>
    [Required]
    [MaxLength(200)]
    [Column("sender_address")]
    public string SenderAddress { get; set; } = string.Empty;

    /// <summary>
    /// 收货地址
    /// </summary>
    [Required]
    [MaxLength(200)]
    [Column("receiver_address")]
    public string ReceiverAddress { get; set; } = string.Empty;

    /// <summary>
    /// 收货人
    /// </summary>
    [Required]
    [MaxLength(50)]
    [Column("receiver_name")]
    public string ReceiverName { get; set; } = string.Empty;

    /// <summary>
    /// 收货人电话
    /// </summary>
    [Required]
    [MaxLength(20)]
    [Column("receiver_phone")]
    public string ReceiverPhone { get; set; } = string.Empty;

    /// <summary>
    /// 物流状态: Pending, InTransit, Delivered
    /// </summary>
    [Required]
    [MaxLength(20)]
    [Column("status")]
    public string Status { get; set; } = "Pending";

    /// <summary>
    /// 发货时间
    /// </summary>
    [Column("shipped_at")]
    public DateTime? ShippedAt { get; set; }

    /// <summary>
    /// 签收时间
    /// </summary>
    [Column("delivered_at")]
    public DateTime? DeliveredAt { get; set; }

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
