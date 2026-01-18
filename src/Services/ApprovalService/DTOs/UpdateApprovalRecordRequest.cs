using System.ComponentModel.DataAnnotations;

namespace Intchain.ApprovalService.DTOs;

/// <summary>
/// 更新审批记录请求
/// </summary>
public class UpdateApprovalRecordRequest
{
    /// <summary>
    /// 审批状态
    /// </summary>
    [MaxLength(20, ErrorMessage = "审批状态长度不能超过20个字符")]
    public string? Status { get; set; }

    /// <summary>
    /// 审批意见
    /// </summary>
    [MaxLength(500, ErrorMessage = "审批意见长度不能超过500个字符")]
    public string? Comments { get; set; }
}
