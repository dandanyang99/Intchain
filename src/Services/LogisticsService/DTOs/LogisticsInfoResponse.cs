namespace Intchain.LogisticsService.DTOs
{
    /// <summary>
    /// 物流信息响应
    /// </summary>
    public class LogisticsInfoResponse
    {
        /// <summary>
        /// 物流信息ID
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 印刷订单ID
        /// </summary>
        public int PrintingOrderId { get; set; }

        /// <summary>
        /// 物流公司
        /// </summary>
        public string LogisticsCompany { get; set; } = string.Empty;

        /// <summary>
        /// 物流单号
        /// </summary>
        public string TrackingNumber { get; set; } = string.Empty;

        /// <summary>
        /// 发货地址
        /// </summary>
        public string SenderAddress { get; set; } = string.Empty;

        /// <summary>
        /// 收货地址
        /// </summary>
        public string ReceiverAddress { get; set; } = string.Empty;

        /// <summary>
        /// 收货人姓名
        /// </summary>
        public string ReceiverName { get; set; } = string.Empty;

        /// <summary>
        /// 收货人电话
        /// </summary>
        public string ReceiverPhone { get; set; } = string.Empty;

        /// <summary>
        /// 物流状态
        /// </summary>
        public string Status { get; set; } = string.Empty;

        /// <summary>
        /// 发货时间
        /// </summary>
        public DateTime? ShippedAt { get; set; }

        /// <summary>
        /// 送达时间
        /// </summary>
        public DateTime? DeliveredAt { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// 更新时间
        /// </summary>
        public DateTime UpdatedAt { get; set; }
    }
}
