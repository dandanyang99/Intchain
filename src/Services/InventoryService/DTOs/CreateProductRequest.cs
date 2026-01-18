using System.ComponentModel.DataAnnotations;

namespace Intchain.InventoryService.DTOs;

/// <summary>
/// 创建产品请求
/// </summary>
public class CreateProductRequest
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

    /// <summary>
    /// 彩票中心ID
    /// </summary>
    [Required(ErrorMessage = "彩票中心ID不能为空")]
    public int LotteryCenterId { get; set; }

    /// <summary>
    /// 印刷厂ID（发布产品时需要指定印刷厂）
    /// </summary>
    [Required(ErrorMessage = "印刷厂ID不能为空")]
    [Range(1, int.MaxValue, ErrorMessage = "印刷厂ID必须大于0")]
    public int PrintingFactoryId { get; set; }
}
