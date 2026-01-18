using Intchain.ApprovalService.DTOs;
using Intchain.ApprovalService.Services;
using Microsoft.AspNetCore.Mvc;

namespace Intchain.ApprovalService.Controllers;

/// <summary>
/// 审批记录控制器
/// </summary>
[ApiController]
[Route("api/approvalrecords")]
public class ApprovalRecordsController : ControllerBase
{
    private readonly IApprovalService _approvalService;

    public ApprovalRecordsController(IApprovalService approvalService)
    {
        _approvalService = approvalService;
    }

    /// <summary>
    /// 获取所有审批记录
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<List<ApprovalRecordResponse>>> GetAllApprovalRecords()
    {
        var records = await _approvalService.GetAllApprovalRecordsAsync();
        return Ok(records);
    }

    /// <summary>
    /// 根据ID获取审批记录
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<ApprovalRecordResponse>> GetApprovalRecord(int id)
    {
        var record = await _approvalService.GetApprovalRecordAsync(id);

        if (record == null)
        {
            return NotFound(new { message = "审批记录不存在" });
        }

        return Ok(record);
    }

    /// <summary>
    /// 根据订单ID获取审批记录
    /// </summary>
    [HttpGet("order/{applicationOrderId}")]
    public async Task<ActionResult<List<ApprovalRecordResponse>>> GetApprovalRecordsByOrder(int applicationOrderId)
    {
        var records = await _approvalService.GetApprovalRecordsByApplicationOrderIdAsync(applicationOrderId);
        return Ok(records);
    }

    /// <summary>
    /// 根据审批人ID获取审批记录
    /// </summary>
    [HttpGet("approver/{approverId}")]
    public async Task<ActionResult<List<ApprovalRecordResponse>>> GetApprovalRecordsByApprover(int approverId)
    {
        var records = await _approvalService.GetApprovalRecordsByApproverIdAsync(approverId);
        return Ok(records);
    }

    /// <summary>
    /// 根据状态获取审批记录
    /// </summary>
    [HttpGet("status/{status}")]
    public async Task<ActionResult<List<ApprovalRecordResponse>>> GetApprovalRecordsByStatus(string status)
    {
        var records = await _approvalService.GetApprovalRecordsByStatusAsync(status);
        return Ok(records);
    }

    /// <summary>
    /// 创建审批记录
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<ApprovalOperationResponse>> CreateApprovalRecord([FromBody] CreateApprovalRecordRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var result = await _approvalService.CreateApprovalRecordAsync(request);

        if (!result.Success)
        {
            return BadRequest(result);
        }

        return CreatedAtAction(nameof(GetApprovalRecord), new { id = result.ApprovalRecordId }, result);
    }

    /// <summary>
    /// 更新审批记录
    /// </summary>
    [HttpPut("{id}")]
    public async Task<ActionResult<ApprovalOperationResponse>> UpdateApprovalRecord(int id, [FromBody] UpdateApprovalRecordRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var result = await _approvalService.UpdateApprovalRecordAsync(id, request);

        if (!result.Success)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }

    /// <summary>
    /// 删除审批记录
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteApprovalRecord(int id)
    {
        var result = await _approvalService.DeleteApprovalRecordAsync(id);

        if (!result)
        {
            return NotFound(new { message = "审批记录不存在" });
        }

        return Ok(new { message = "审批记录删除成功" });
    }
}
