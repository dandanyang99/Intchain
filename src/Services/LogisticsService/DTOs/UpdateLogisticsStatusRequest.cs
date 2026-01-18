using System.ComponentModel.DataAnnotations;

namespace Intchain.LogisticsService.DTOs
{
    /// <summary>
    /// 更新物流状态请求
    /// </summary>
    public class UpdateLogisticsStatusRequest
    {
        /// <summary>
        /// 物流状态 (Pending, InTransit, Delivered)
        /// </summary>
        [Required(ErrorMessage = "物流状态不能为空")]
        [MaxLength(20, ErrorMessage = "物流状态长度不能超过20个字符")]
        public string Status { get; set; } = string.Empty;
    }
}
