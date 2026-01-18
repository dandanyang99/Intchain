namespace Intchain.ApprovalService.DTOs;

/// <summary>
/// 审批操作响应
/// </summary>
public class ApprovalOperationResponse
{
    /// <summary>
    /// 操作是否成功
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// 操作消息
    /// </summary>
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// 审批记录ID
    /// </summary>
    public int? ApprovalRecordId { get; set; }

    /// <summary>
    /// 申请订单ID
    /// </summary>
    public int? ApplicationOrderId { get; set; }

    /// <summary>
    /// 当前审批状态
    /// </summary>
    public string? CurrentStatus { get; set; }
}
