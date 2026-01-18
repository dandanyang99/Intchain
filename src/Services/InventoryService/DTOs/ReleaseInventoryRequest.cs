using System.ComponentModel.DataAnnotations;

namespace Intchain.InventoryService.DTOs;

/// <summary>
/// 释放库存请求
/// </summary>
public class ReleaseInventoryRequest
{
    /// <summary>
    /// 产品ID
    /// </summary>
    [Required(ErrorMessage = "产品ID不能为空")]
    public int ProductId { get; set; }

    /// <summary>
    /// 数量
    /// </summary>
    [Required(ErrorMessage = "数量不能为空")]
    [Range(1, int.MaxValue, ErrorMessage = "数量必须大于0")]
    public int Quantity { get; set; }

    /// <summary>
    /// 订单ID
    /// </summary>
    [Required(ErrorMessage = "订单ID不能为空")]
    [MaxLength(100, ErrorMessage = "订单ID长度不能超过100个字符")]
    public string OrderId { get; set; } = string.Empty;
}
