using System.ComponentModel.DataAnnotations;

namespace Intchain.LogisticsService.DTOs
{
    /// <summary>
    /// 创建物流信息请求
    /// </summary>
    public class CreateLogisticsInfoRequest
    {
        /// <summary>
        /// 印刷订单ID
        /// </summary>
        [Required(ErrorMessage = "印刷订单ID不能为空")]
        public int PrintingOrderId { get; set; }

        /// <summary>
        /// 物流公司
        /// </summary>
        [Required(ErrorMessage = "物流公司不能为空")]
        [MaxLength(100, ErrorMessage = "物流公司名称长度不能超过100个字符")]
        public string LogisticsCompany { get; set; } = string.Empty;

        /// <summary>
        /// 物流单号
        /// </summary>
        [Required(ErrorMessage = "物流单号不能为空")]
        [MaxLength(100, ErrorMessage = "物流单号长度不能超过100个字符")]
        public string TrackingNumber { get; set; } = string.Empty;

        /// <summary>
        /// 发货地址
        /// </summary>
        [Required(ErrorMessage = "发货地址不能为空")]
        [MaxLength(200, ErrorMessage = "发货地址长度不能超过200个字符")]
        public string SenderAddress { get; set; } = string.Empty;

        /// <summary>
        /// 收货地址
        /// </summary>
        [Required(ErrorMessage = "收货地址不能为空")]
        [MaxLength(200, ErrorMessage = "收货地址长度不能超过200个字符")]
        public string ReceiverAddress { get; set; } = string.Empty;

        /// <summary>
        /// 收货人姓名
        /// </summary>
        [Required(ErrorMessage = "收货人姓名不能为空")]
        [MaxLength(50, ErrorMessage = "收货人姓名长度不能超过50个字符")]
        public string ReceiverName { get; set; } = string.Empty;

        /// <summary>
        /// 收货人电话
        /// </summary>
        [Required(ErrorMessage = "收货人电话不能为空")]
        [MaxLength(20, ErrorMessage = "收货人电话长度不能超过20个字符")]
        public string ReceiverPhone { get; set; } = string.Empty;
    }
}
