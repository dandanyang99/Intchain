using Intchain.ApprovalService.Data;
using Intchain.ApprovalService.DTOs;
using Intchain.ApprovalService.Models;
using Intchain.ApprovalService.Constants;
using Microsoft.EntityFrameworkCore;

namespace Intchain.ApprovalService.Services;

/// <summary>
/// 审批服务实现
/// </summary>
public class ApprovalService : IApprovalService
{
    private readonly ApprovalDbContext _context;

    public ApprovalService(ApprovalDbContext context)
    {
        _context = context;
    }

    public async Task<ApprovalOperationResponse> CreateApprovalRecordAsync(CreateApprovalRecordRequest request)
    {
        // Validate status
        if (request.Status != ApprovalStatus.Pending &&
            request.Status != ApprovalStatus.Approved &&
            request.Status != ApprovalStatus.Rejected)
        {
            return new ApprovalOperationResponse
            {
                Success = false,
                Message = $"无效的审批状态: {request.Status}"
            };
        }

        // Check for duplicate approvals (prevent multiple final approvals for same order)
        if (request.Status == ApprovalStatus.Approved || request.Status == ApprovalStatus.Rejected)
        {
            var existingApproval = await _context.ApprovalRecords
                .FirstOrDefaultAsync(a => a.ApplicationOrderId == request.ApplicationOrderId
                    && (a.Status == ApprovalStatus.Approved || a.Status == ApprovalStatus.Rejected));

            if (existingApproval != null)
            {
                return new ApprovalOperationResponse
                {
                    Success = false,
                    Message = "该订单已经存在审批记录",
                    ApplicationOrderId = request.ApplicationOrderId,
                    CurrentStatus = existingApproval.Status
                };
            }
        }

        var record = new ApprovalRecord
        {
            ApplicationOrderId = request.ApplicationOrderId,
            ApproverId = request.ApproverId,
            Status = request.Status,
            Comments = request.Comments,
            ApprovedAt = (request.Status == ApprovalStatus.Approved || request.Status == ApprovalStatus.Rejected)
                ? DateTime.UtcNow
                : null,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.ApprovalRecords.Add(record);
        await _context.SaveChangesAsync();

        return new ApprovalOperationResponse
        {
            Success = true,
            Message = "审批记录创建成功",
            ApprovalRecordId = record.Id,
            ApplicationOrderId = record.ApplicationOrderId,
            CurrentStatus = record.Status
        };
    }

    public async Task<ApprovalOperationResponse> UpdateApprovalRecordAsync(int id, UpdateApprovalRecordRequest request)
    {
        var record = await _context.ApprovalRecords.FindAsync(id);

        if (record == null)
        {
            return new ApprovalOperationResponse
            {
                Success = false,
                Message = "审批记录不存在"
            };
        }

        // Update status if provided
        if (!string.IsNullOrEmpty(request.Status))
        {
            // Validate status
            if (request.Status != ApprovalStatus.Pending &&
                request.Status != ApprovalStatus.Approved &&
                request.Status != ApprovalStatus.Rejected)
            {
                return new ApprovalOperationResponse
                {
                    Success = false,
                    Message = $"无效的审批状态: {request.Status}",
                    ApprovalRecordId = record.Id,
                    ApplicationOrderId = record.ApplicationOrderId,
                    CurrentStatus = record.Status
                };
            }

            record.Status = request.Status;

            // Update ApprovedAt if status changed to Approved or Rejected
            if ((request.Status == ApprovalStatus.Approved || request.Status == ApprovalStatus.Rejected)
                && record.ApprovedAt == null)
            {
                record.ApprovedAt = DateTime.UtcNow;
            }
        }

        // Update comments if provided
        if (request.Comments != null)
        {
            record.Comments = request.Comments;
        }

        record.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return new ApprovalOperationResponse
        {
            Success = true,
            Message = "审批记录更新成功",
            ApprovalRecordId = record.Id,
            ApplicationOrderId = record.ApplicationOrderId,
            CurrentStatus = record.Status
        };
    }

    public async Task<ApprovalRecordResponse?> GetApprovalRecordAsync(int id)
    {
        var record = await _context.ApprovalRecords.FindAsync(id);

        return record == null ? null : MapToResponse(record);
    }

    public async Task<List<ApprovalRecordResponse>> GetAllApprovalRecordsAsync()
    {
        var records = await _context.ApprovalRecords.ToListAsync();

        return records.Select(MapToResponse).ToList();
    }

    public async Task<List<ApprovalRecordResponse>> GetApprovalRecordsByApplicationOrderIdAsync(int applicationOrderId)
    {
        var records = await _context.ApprovalRecords
            .Where(a => a.ApplicationOrderId == applicationOrderId)
            .ToListAsync();

        return records.Select(MapToResponse).ToList();
    }

    public async Task<List<ApprovalRecordResponse>> GetApprovalRecordsByApproverIdAsync(int approverId)
    {
        var records = await _context.ApprovalRecords
            .Where(a => a.ApproverId == approverId)
            .ToListAsync();

        return records.Select(MapToResponse).ToList();
    }

    public async Task<List<ApprovalRecordResponse>> GetApprovalRecordsByStatusAsync(string status)
    {
        var records = await _context.ApprovalRecords
            .Where(a => a.Status == status)
            .ToListAsync();

        return records.Select(MapToResponse).ToList();
    }

    public async Task<bool> DeleteApprovalRecordAsync(int id)
    {
        var record = await _context.ApprovalRecords.FindAsync(id);

        if (record == null)
        {
            return false;
        }

        _context.ApprovalRecords.Remove(record);
        await _context.SaveChangesAsync();

        return true;
    }

    private static ApprovalRecordResponse MapToResponse(ApprovalRecord record)
    {
        return new ApprovalRecordResponse
        {
            Id = record.Id,
            ApplicationOrderId = record.ApplicationOrderId,
            ApproverId = record.ApproverId,
            Status = record.Status,
            Comments = record.Comments,
            ApprovedAt = record.ApprovedAt,
            CreatedAt = record.CreatedAt,
            UpdatedAt = record.UpdatedAt
        };
    }
}
