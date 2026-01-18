using System.ComponentModel.DataAnnotations;

namespace Intchain.OrderService.DTOs;

/// <summary>
/// 拒绝申请订单请求
/// </summary>
public class RejectApplicationOrderRequest
{
    /// <summary>
    /// 拒绝原因
    /// </summary>
    [Required(ErrorMessage = "拒绝原因不能为空")]
    [MaxLength(500, ErrorMessage = "拒绝原因长度不能超过500个字符")]
    public string RejectionReason { get; set; } = string.Empty;
}
