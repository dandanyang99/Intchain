using System.ComponentModel.DataAnnotations;

namespace Intchain.OrderService.DTOs;

/// <summary>
/// 审批申请订单请求
/// </summary>
public class ApproveApplicationOrderRequest
{
    /// <summary>
    /// 审批数量（可能与申请数量不同）
    /// </summary>
    [Required(ErrorMessage = "审批数量不能为空")]
    [Range(1, int.MaxValue, ErrorMessage = "审批数量必须大于0")]
    public int ApprovedQuantity { get; set; }

    /// <summary>
    /// 印刷厂ID
    /// </summary>
    [Required(ErrorMessage = "印刷厂ID不能为空")]
    public int PrintingFactoryId { get; set; }

    /// <summary>
    /// 审批备注
    /// </summary>
    [MaxLength(500, ErrorMessage = "审批备注长度不能超过500个字符")]
    public string? ApprovalRemarks { get; set; }
}
