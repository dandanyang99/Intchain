using Intchain.LogisticsService.Constants;
using Intchain.LogisticsService.Data;
using Intchain.LogisticsService.DTOs;
using Intchain.LogisticsService.Models;
using Microsoft.EntityFrameworkCore;

namespace Intchain.LogisticsService.Services
{
    /// <summary>
    /// 物流服务实现
    /// </summary>
    public class LogisticsService : ILogisticsService
    {
        private readonly LogisticsDbContext _context;

        public LogisticsService(LogisticsDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// 创建物流信息
        /// </summary>
        public async Task<LogisticsOperationResponse> CreateLogisticsInfoAsync(CreateLogisticsInfoRequest request)
        {
            // 检查物流单号是否已存在
            var existingTracking = await _context.LogisticsInfos
                .FirstOrDefaultAsync(l => l.TrackingNumber == request.TrackingNumber);

            if (existingTracking != null)
            {
                return new LogisticsOperationResponse
                {
                    Success = false,
                    Message = "物流单号已存在，请检查后重试"
                };
            }

            var logisticsInfo = new LogisticsInfo
            {
                PrintingOrderId = request.PrintingOrderId,
                LogisticsCompany = request.LogisticsCompany,
                TrackingNumber = request.TrackingNumber,
                SenderAddress = request.SenderAddress,
                ReceiverAddress = request.ReceiverAddress,
                ReceiverName = request.ReceiverName,
                ReceiverPhone = request.ReceiverPhone,
                Status = LogisticsStatus.Pending,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.LogisticsInfos.Add(logisticsInfo);
            await _context.SaveChangesAsync();

            return new LogisticsOperationResponse
            {
                Success = true,
                Message = "物流信息创建成功",
                LogisticsInfoId = logisticsInfo.Id,
                TrackingNumber = logisticsInfo.TrackingNumber,
                CurrentStatus = logisticsInfo.Status
            };
        }

        /// <summary>
        /// 更新物流状态
        /// </summary>
        public async Task<LogisticsOperationResponse> UpdateLogisticsStatusAsync(int id, UpdateLogisticsStatusRequest request)
        {
            var logisticsInfo = await _context.LogisticsInfos.FindAsync(id);

            if (logisticsInfo == null)
            {
                return new LogisticsOperationResponse
                {
                    Success = false,
                    Message = "物流信息不存在"
                };
            }

            // 验证状态值
            if (request.Status != LogisticsStatus.Pending &&
                request.Status != LogisticsStatus.InTransit &&
                request.Status != LogisticsStatus.Delivered)
            {
                return new LogisticsOperationResponse
                {
                    Success = false,
                    Message = "无效的物流状态"
                };
            }

            // 更新状态
            logisticsInfo.Status = request.Status;
            logisticsInfo.UpdatedAt = DateTime.UtcNow;

            // 根据状态设置时间戳
            if (request.Status == LogisticsStatus.InTransit && logisticsInfo.ShippedAt == null)
            {
                logisticsInfo.ShippedAt = DateTime.UtcNow;
            }
            else if (request.Status == LogisticsStatus.Delivered && logisticsInfo.DeliveredAt == null)
            {
                logisticsInfo.DeliveredAt = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync();

            return new LogisticsOperationResponse
            {
                Success = true,
                Message = "物流状态更新成功",
                LogisticsInfoId = logisticsInfo.Id,
                TrackingNumber = logisticsInfo.TrackingNumber,
                CurrentStatus = logisticsInfo.Status
            };
        }

        /// <summary>
        /// 根据ID获取物流信息
        /// </summary>
        public async Task<LogisticsInfoResponse?> GetLogisticsInfoAsync(int id)
        {
            var logisticsInfo = await _context.LogisticsInfos.FindAsync(id);

            if (logisticsInfo == null)
                return null;

            return MapToResponse(logisticsInfo);
        }

        /// <summary>
        /// 获取所有物流信息
        /// </summary>
        public async Task<List<LogisticsInfoResponse>> GetAllLogisticsInfoAsync()
        {
            var logisticsList = await _context.LogisticsInfos
                .OrderByDescending(l => l.CreatedAt)
                .ToListAsync();

            return logisticsList.Select(MapToResponse).ToList();
        }

        /// <summary>
        /// 根据物流单号获取物流信息
        /// </summary>
        public async Task<LogisticsInfoResponse?> GetLogisticsInfoByTrackingNumberAsync(string trackingNumber)
        {
            var logisticsInfo = await _context.LogisticsInfos
                .FirstOrDefaultAsync(l => l.TrackingNumber == trackingNumber);

            if (logisticsInfo == null)
                return null;

            return MapToResponse(logisticsInfo);
        }

        /// <summary>
        /// 根据印刷订单ID获取物流信息
        /// </summary>
        public async Task<LogisticsInfoResponse?> GetLogisticsInfoByPrintingOrderIdAsync(int printingOrderId)
        {
            var logisticsInfo = await _context.LogisticsInfos
                .FirstOrDefaultAsync(l => l.PrintingOrderId == printingOrderId);

            if (logisticsInfo == null)
                return null;

            return MapToResponse(logisticsInfo);
        }

        /// <summary>
        /// 根据状态获取物流信息列表
        /// </summary>
        public async Task<List<LogisticsInfoResponse>> GetLogisticsInfoByStatusAsync(string status)
        {
            var logisticsList = await _context.LogisticsInfos
                .Where(l => l.Status == status)
                .OrderByDescending(l => l.CreatedAt)
                .ToListAsync();

            return logisticsList.Select(MapToResponse).ToList();
        }

        /// <summary>
        /// 删除物流信息
        /// </summary>
        public async Task<bool> DeleteLogisticsInfoAsync(int id)
        {
            var logisticsInfo = await _context.LogisticsInfos.FindAsync(id);

            if (logisticsInfo == null)
                return false;

            _context.LogisticsInfos.Remove(logisticsInfo);
            await _context.SaveChangesAsync();

            return true;
        }

        /// <summary>
        /// 映射实体到响应DTO
        /// </summary>
        private static LogisticsInfoResponse MapToResponse(LogisticsInfo logisticsInfo)
        {
            return new LogisticsInfoResponse
            {
                Id = logisticsInfo.Id,
                PrintingOrderId = logisticsInfo.PrintingOrderId,
                LogisticsCompany = logisticsInfo.LogisticsCompany,
                TrackingNumber = logisticsInfo.TrackingNumber,
                SenderAddress = logisticsInfo.SenderAddress,
                ReceiverAddress = logisticsInfo.ReceiverAddress,
                ReceiverName = logisticsInfo.ReceiverName,
                ReceiverPhone = logisticsInfo.ReceiverPhone,
                Status = logisticsInfo.Status,
                ShippedAt = logisticsInfo.ShippedAt,
                DeliveredAt = logisticsInfo.DeliveredAt,
                CreatedAt = logisticsInfo.CreatedAt,
                UpdatedAt = logisticsInfo.UpdatedAt
            };
        }
    }
}
