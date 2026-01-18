using System.ComponentModel.DataAnnotations;

namespace Intchain.ApprovalService.DTOs;

/// <summary>
/// 创建审批记录请求
/// </summary>
public class CreateApprovalRecordRequest
{
    /// <summary>
    /// 申请订单ID
    /// </summary>
    [Required(ErrorMessage = "申请订单ID不能为空")]
    [Range(1, int.MaxValue, ErrorMessage = "申请订单ID必须大于0")]
    public int ApplicationOrderId { get; set; }

    /// <summary>
    /// 审批人ID
    /// </summary>
    [Required(ErrorMessage = "审批人ID不能为空")]
    [Range(1, int.MaxValue, ErrorMessage = "审批人ID必须大于0")]
    public int ApproverId { get; set; }

    /// <summary>
    /// 审批状态
    /// </summary>
    [Required(ErrorMessage = "审批状态不能为空")]
    [MaxLength(20, ErrorMessage = "审批状态长度不能超过20个字符")]
    public string Status { get; set; } = string.Empty;

    /// <summary>
    /// 审批意见
    /// </summary>
    [MaxLength(500, ErrorMessage = "审批意见长度不能超过500个字符")]
    public string? Comments { get; set; }
}
