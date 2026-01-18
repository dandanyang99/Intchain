using System.ComponentModel.DataAnnotations;

namespace Intchain.UserService.DTOs;

/// <summary>
/// 登录请求DTO
/// </summary>
public class LoginRequest
{
    /// <summary>
    /// 用户名
    /// </summary>
    [Required(ErrorMessage = "用户名不能为空")]
    [MaxLength(50, ErrorMessage = "用户名长度不能超过50个字符")]
    public string Username { get; set; } = string.Empty;

    /// <summary>
    /// 密码
    /// </summary>
    [Required(ErrorMessage = "密码不能为空")]
    [MinLength(6, ErrorMessage = "密码长度不能少于6个字符")]
    public string Password { get; set; } = string.Empty;
}
