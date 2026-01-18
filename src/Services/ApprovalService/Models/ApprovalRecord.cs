using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Intchain.ApprovalService.Models;

/// <summary>
/// 审批记录实体
/// </summary>
[Table("approval_records")]
public class ApprovalRecord
{
    /// <summary>
    /// 审批记录ID
    /// </summary>
    [Key]
    [Column("id")]
    public int Id { get; set; }

    /// <summary>
    /// 申请订单ID
    /// </summary>
    [Required]
    [Column("application_order_id")]
    public int ApplicationOrderId { get; set; }

    /// <summary>
    /// 审批人用户ID
    /// </summary>
    [Required]
    [Column("approver_id")]
    public int ApproverId { get; set; }

    /// <summary>
    /// 审批状态: Pending, Approved, Rejected
    /// </summary>
    [Required]
    [MaxLength(20)]
    [Column("status")]
    public string Status { get; set; } = "Pending";

    /// <summary>
    /// 审批意见
    /// </summary>
    [MaxLength(500)]
    [Column("comments")]
    public string? Comments { get; set; }

    /// <summary>
    /// 审批时间
    /// </summary>
    [Column("approved_at")]
    public DateTime? ApprovedAt { get; set; }

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
