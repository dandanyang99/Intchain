namespace Intchain.LogisticsService.DTOs
{
    /// <summary>
    /// 物流操作响应
    /// </summary>
    public class LogisticsOperationResponse
    {
        /// <summary>
        /// 操作是否成功
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// 响应消息
        /// </summary>
        public string Message { get; set; } = string.Empty;

        /// <summary>
        /// 物流信息ID
        /// </summary>
        public int? LogisticsInfoId { get; set; }

        /// <summary>
        /// 物流单号
        /// </summary>
        public string? TrackingNumber { get; set; }

        /// <summary>
        /// 当前状态
        /// </summary>
        public string? CurrentStatus { get; set; }
    }
}
