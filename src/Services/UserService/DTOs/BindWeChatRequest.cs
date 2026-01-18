using System.ComponentModel.DataAnnotations;

namespace Intchain.UserService.DTOs;

/// <summary>
/// 绑定微信请求DTO（注册时或后续绑定使用）
/// </summary>
public class BindWeChatRequest
{
    /// <summary>
    /// 微信登录凭证code
    /// </summary>
    [Required(ErrorMessage = "微信登录凭证不能为空")]
    [MaxLength(100)]
    public string Code { get; set; } = string.Empty;
}
