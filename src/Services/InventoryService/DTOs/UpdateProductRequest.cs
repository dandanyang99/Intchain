using System.ComponentModel.DataAnnotations;

namespace Intchain.InventoryService.DTOs;

/// <summary>
/// 更新产品请求
/// </summary>
public class UpdateProductRequest
{
    /// <summary>
    /// 产品名称
    /// </summary>
    [Required(ErrorMessage = "产品名称不能为空")]
    [MaxLength(100, ErrorMessage = "产品名称长度不能超过100个字符")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 单价
    /// </summary>
    [Required(ErrorMessage = "单价不能为空")]
    [Range(0.01, double.MaxValue, ErrorMessage = "单价必须大于0")]
    public decimal UnitPrice { get; set; }

    /// <summary>
    /// 总库存
    /// </summary>
    [Required(ErrorMessage = "总库存不能为空")]
    [Range(0, int.MaxValue, ErrorMessage = "总库存不能为负数")]
    public int TotalStock { get; set; }
}
