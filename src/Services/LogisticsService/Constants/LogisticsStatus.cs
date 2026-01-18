namespace Intchain.LogisticsService.Constants
{
    /// <summary>
    /// 物流状态常量
    /// </summary>
    public static class LogisticsStatus
    {
        /// <summary>
        /// 待发货
        /// </summary>
        public const string Pending = "Pending";

        /// <summary>
        /// 运输中
        /// </summary>
        public const string InTransit = "InTransit";

        /// <summary>
        /// 已送达
        /// </summary>
        public const string Delivered = "Delivered";
    }
}
