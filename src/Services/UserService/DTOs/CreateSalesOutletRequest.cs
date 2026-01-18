using System.ComponentModel.DataAnnotations;

namespace Intchain.UserService.DTOs;

/// <summary>
/// 创建销售网点请求
/// </summary>
public class CreateSalesOutletRequest
{
    /// <summary>
    /// 网点名称
    /// </summary>
    [Required(ErrorMessage = "网点名称不能为空")]
    [MaxLength(100, ErrorMessage = "网点名称长度不能超过100个字符")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 所属彩票中心ID
    /// </summary>
    [Required(ErrorMessage = "所属彩票中心ID不能为空")]
    [Range(1, int.MaxValue, ErrorMessage = "所属彩票中心ID必须大于0")]
    public int LotteryCenterId { get; set; }

    /// <summary>
    /// 联系人
    /// </summary>
    [Required(ErrorMessage = "联系人不能为空")]
    [MaxLength(50, ErrorMessage = "联系人长度不能超过50个字符")]
    public string ContactPerson { get; set; } = string.Empty;

    /// <summary>
    /// 联系电话
    /// </summary>
    [Required(ErrorMessage = "联系电话不能为空")]
    [MaxLength(20, ErrorMessage = "联系电话长度不能超过20个字符")]
    [Phone(ErrorMessage = "联系电话格式不正确")]
    public string ContactPhone { get; set; } = string.Empty;

    /// <summary>
    /// 地址
    /// </summary>
    [Required(ErrorMessage = "地址不能为空")]
    [MaxLength(200, ErrorMessage = "地址长度不能超过200个字符")]
    public string Address { get; set; } = string.Empty;

    /// <summary>
    /// 默认用户名（可选，如果不提供则自动生成）
    /// </summary>
    [MaxLength(50, ErrorMessage = "用户名长度不能超过50个字符")]
    public string? DefaultUsername { get; set; }

    /// <summary>
    /// 默认密码（可选，如果不提供则自动生成）
    /// </summary>
    [MaxLength(50, ErrorMessage = "密码长度不能超过50个字符")]
    public string? DefaultPassword { get; set; }
}
