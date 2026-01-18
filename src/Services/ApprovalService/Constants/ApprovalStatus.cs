namespace Intchain.ApprovalService.Constants;

/// <summary>
/// 审批状态常量
/// </summary>
public static class ApprovalStatus
{
    /// <summary>
    /// 待审批
    /// </summary>
    public const string Pending = "Pending";

    /// <summary>
    /// 审批通过
    /// </summary>
    public const string Approved = "Approved";

    /// <summary>
    /// 审批拒绝
    /// </summary>
    public const string Rejected = "Rejected";
}
