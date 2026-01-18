using System.ComponentModel.DataAnnotations;

namespace Intchain.UserService.DTOs;

/// <summary>
/// 微信登录请求DTO
/// </summary>
public class WeChatLoginRequest
{
    /// <summary>
    /// 微信登录凭证code
    /// </summary>
    [Required(ErrorMessage = "微信登录凭证不能为空")]
    [MaxLength(100, ErrorMessage = "登录凭证格式错误")]
    public string Code { get; set; } = string.Empty;
}
