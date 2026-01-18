using Intchain.ApprovalService.DTOs;

namespace Intchain.ApprovalService.Services;

/// <summary>
/// 审批服务接口
/// </summary>
public interface IApprovalService
{
    /// <summary>
    /// 创建审批记录
    /// </summary>
    Task<ApprovalOperationResponse> CreateApprovalRecordAsync(CreateApprovalRecordRequest request);

    /// <summary>
    /// 更新审批记录
    /// </summary>
    Task<ApprovalOperationResponse> UpdateApprovalRecordAsync(int id, UpdateApprovalRecordRequest request);

    /// <summary>
    /// 获取审批记录
    /// </summary>
    Task<ApprovalRecordResponse?> GetApprovalRecordAsync(int id);

    /// <summary>
    /// 获取所有审批记录
    /// </summary>
    Task<List<ApprovalRecordResponse>> GetAllApprovalRecordsAsync();

    /// <summary>
    /// 根据申请订单ID获取审批记录
    /// </summary>
    Task<List<ApprovalRecordResponse>> GetApprovalRecordsByApplicationOrderIdAsync(int applicationOrderId);

    /// <summary>
    /// 根据审批人ID获取审批记录
    /// </summary>
    Task<List<ApprovalRecordResponse>> GetApprovalRecordsByApproverIdAsync(int approverId);

    /// <summary>
    /// 根据状态获取审批记录
    /// </summary>
    Task<List<ApprovalRecordResponse>> GetApprovalRecordsByStatusAsync(string status);

    /// <summary>
    /// 删除审批记录
    /// </summary>
    Task<bool> DeleteApprovalRecordAsync(int id);
}
