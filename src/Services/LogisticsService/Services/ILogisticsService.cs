using Intchain.LogisticsService.DTOs;

namespace Intchain.LogisticsService.Services
{
    /// <summary>
    /// 物流服务接口
    /// </summary>
    public interface ILogisticsService
    {
        /// <summary>
        /// 创建物流信息
        /// </summary>
        Task<LogisticsOperationResponse> CreateLogisticsInfoAsync(CreateLogisticsInfoRequest request);

        /// <summary>
        /// 更新物流状态
        /// </summary>
        Task<LogisticsOperationResponse> UpdateLogisticsStatusAsync(int id, UpdateLogisticsStatusRequest request);

        /// <summary>
        /// 根据ID获取物流信息
        /// </summary>
        Task<LogisticsInfoResponse?> GetLogisticsInfoAsync(int id);

        /// <summary>
        /// 获取所有物流信息
        /// </summary>
        Task<List<LogisticsInfoResponse>> GetAllLogisticsInfoAsync();

        /// <summary>
        /// 根据物流单号获取物流信息
        /// </summary>
        Task<LogisticsInfoResponse?> GetLogisticsInfoByTrackingNumberAsync(string trackingNumber);

        /// <summary>
        /// 根据印刷订单ID获取物流信息
        /// </summary>
        Task<LogisticsInfoResponse?> GetLogisticsInfoByPrintingOrderIdAsync(int printingOrderId);

        /// <summary>
        /// 根据状态获取物流信息列表
        /// </summary>
        Task<List<LogisticsInfoResponse>> GetLogisticsInfoByStatusAsync(string status);

        /// <summary>
        /// 删除物流信息
        /// </summary>
        Task<bool> DeleteLogisticsInfoAsync(int id);
    }
}
