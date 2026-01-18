using System.ComponentModel.DataAnnotations;

namespace Intchain.OrderService.DTOs;

/// <summary>
/// 创建申请订单请求
/// </summary>
public class CreateApplicationOrderRequest
{
    /// <summary>
    /// 销售网点ID
    /// </summary>
    [Required(ErrorMessage = "销售网点ID不能为空")]
    public int SalesOutletId { get; set; }

    /// <summary>
    /// 彩票中心ID
    /// </summary>
    [Required(ErrorMessage = "彩票中心ID不能为空")]
    public int LotteryCenterId { get; set; }

    /// <summary>
    /// 彩票产品ID
    /// </summary>
    [Required(ErrorMessage = "彩票产品ID不能为空")]
    public int ProductId { get; set; }

    /// <summary>
    /// 申请数量
    /// </summary>
    [Required(ErrorMessage = "申请数量不能为空")]
    [Range(1, int.MaxValue, ErrorMessage = "申请数量必须大于0")]
    public int Quantity { get; set; }

    /// <summary>
    /// 备注
    /// </summary>
    [MaxLength(500, ErrorMessage = "备注长度不能超过500个字符")]
    public string? Remarks { get; set; }
}
