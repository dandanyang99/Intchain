using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Intchain.OrderService.Models;

/// <summary>
/// 订单状态转换历史记录
/// </summary>
[Table("order_status_history")]
public class OrderStatusHistory
{
    /// <summary>
    /// 历史记录ID
    /// </summary>
    [Key]
    [Column("id")]
    public int Id { get; set; }

    /// <summary>
    /// 订单类型（Application/Printing）
    /// </summary>
    [Required]
    [MaxLength(20)]
    [Column("order_type")]
    public string OrderType { get; set; } = string.Empty;

    /// <summary>
    /// 订单ID
    /// </summary>
    [Required]
    [Column("order_id")]
    public int OrderId { get; set; }

    /// <summary>
    /// 原状态
    /// </summary>
    [Required]
    [MaxLength(50)]
    [Column("from_status")]
    public string FromStatus { get; set; } = string.Empty;

    /// <summary>
    /// 新状态
    /// </summary>
    [Required]
    [MaxLength(50)]
    [Column("to_status")]
    public string ToStatus { get; set; } = string.Empty;

    /// <summary>
    /// 操作人ID（可选）
    /// </summary>
    [Column("operator_id")]
    public int? OperatorId { get; set; }

    /// <summary>
    /// 操作人姓名（可选）
    /// </summary>
    [MaxLength(100)]
    [Column("operator_name")]
    public string? OperatorName { get; set; }

    /// <summary>
    /// 转换原因/备注（可选）
    /// </summary>
    [MaxLength(500)]
    [Column("reason")]
    public string? Reason { get; set; }

    /// <summary>
    /// 创建时间
    /// </summary>
    [Required]
    [Column("created_at")]
    public DateTime CreatedAt { get; set; }
}
