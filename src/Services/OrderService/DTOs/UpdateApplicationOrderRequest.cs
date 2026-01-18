using System.ComponentModel.DataAnnotations;

namespace Intchain.OrderService.DTOs;

/// <summary>
/// 更新申请订单请求
/// </summary>
public class UpdateApplicationOrderRequest
{
    /// <summary>
    /// 申请数量
    /// </summary>
    [Range(1, int.MaxValue, ErrorMessage = "申请数量必须大于0")]
    public int? Quantity { get; set; }

    /// <summary>
    /// 备注
    /// </summary>
    [MaxLength(500, ErrorMessage = "备注长度不能超过500个字符")]
    public string? Remarks { get; set; }
}
