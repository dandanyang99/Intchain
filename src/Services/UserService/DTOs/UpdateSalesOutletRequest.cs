using System.ComponentModel.DataAnnotations;

namespace Intchain.UserService.DTOs;

/// <summary>
/// 更新销售网点请求
/// </summary>
public class UpdateSalesOutletRequest
{
    /// <summary>
    /// 网点名称
    /// </summary>
    [MaxLength(100, ErrorMessage = "网点名称长度不能超过100个字符")]
    public string? Name { get; set; }

    /// <summary>
    /// 联系人
    /// </summary>
    [MaxLength(50, ErrorMessage = "联系人长度不能超过50个字符")]
    public string? ContactPerson { get; set; }

    /// <summary>
    /// 联系电话
    /// </summary>
    [MaxLength(20, ErrorMessage = "联系电话长度不能超过20个字符")]
    [Phone(ErrorMessage = "联系电话格式不正确")]
    public string? ContactPhone { get; set; }

    /// <summary>
    /// 地址
    /// </summary>
    [MaxLength(200, ErrorMessage = "地址长度不能超过200个字符")]
    public string? Address { get; set; }

    /// <summary>
    /// 是否激活
    /// </summary>
    public bool? IsActive { get; set; }
}
