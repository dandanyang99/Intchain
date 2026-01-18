namespace Intchain.ApprovalService.DTOs;

/// <summary>
/// 审批记录响应
/// </summary>
public class ApprovalRecordResponse
{
    /// <summary>
    /// 审批记录ID
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// 申请订单ID
    /// </summary>
    public int ApplicationOrderId { get; set; }

    /// <summary>
    /// 审批人ID
    /// </summary>
    public int ApproverId { get; set; }

    /// <summary>
    /// 审批状态
    /// </summary>
    public string Status { get; set; } = string.Empty;

    /// <summary>
    /// 审批意见
    /// </summary>
    public string? Comments { get; set; }

    /// <summary>
    /// 审批时间
    /// </summary>
    public DateTime? ApprovedAt { get; set; }

    /// <summary>
    /// 创建时间
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// 更新时间
    /// </summary>
    public DateTime UpdatedAt { get; set; }
}
