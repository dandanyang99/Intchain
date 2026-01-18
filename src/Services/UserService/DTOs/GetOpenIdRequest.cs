using System.ComponentModel.DataAnnotations;

namespace Intchain.UserService.DTOs;

/// <summary>
/// 获取OpenId请求DTO
/// </summary>
public class GetOpenIdRequest
{
    /// <summary>
    /// 微信登录凭证code
    /// </summary>
    [Required(ErrorMessage = "微信登录凭证不能为空")]
    [MaxLength(100, ErrorMessage = "微信登录凭证长度不能超过100个字符")]
    public string Code { get; set; } = string.Empty;
}
