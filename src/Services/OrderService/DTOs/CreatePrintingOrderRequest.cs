using System.ComponentModel.DataAnnotations;

namespace Intchain.OrderService.DTOs;

/// <summary>
/// 创建印刷订单请求
/// </summary>
public class CreatePrintingOrderRequest
{
    /// <summary>
    /// 关联的申请订单ID（可选，发布产品时创建的印刷订单可能没有关联申请订单）
    /// </summary>
    public int? ApplicationOrderId { get; set; }

    /// <summary>
    /// 印刷厂ID
    /// </summary>
    [Required(ErrorMessage = "印刷厂ID不能为空")]
    public int PrintingFactoryId { get; set; }

    /// <summary>
    /// 彩票产品ID
    /// </summary>
    [Required(ErrorMessage = "彩票产品ID不能为空")]
    public int ProductId { get; set; }

    /// <summary>
    /// 印刷数量
    /// </summary>
    [Required(ErrorMessage = "印刷数量不能为空")]
    [Range(1, int.MaxValue, ErrorMessage = "印刷数量必须大于0")]
    public int Quantity { get; set; }

    /// <summary>
    /// 备注
    /// </summary>
    [MaxLength(500, ErrorMessage = "备注长度不能超过500个字符")]
    public string? Remarks { get; set; }
}
