using System.ComponentModel.DataAnnotations;

namespace Intchain.UserService.DTOs;

/// <summary>
/// 注册请求DTO
/// </summary>
public class RegisterRequest
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

    /// <summary>
    /// 角色: Admin, LotteryCenter, PrintingFactory, SalesOutlet
    /// </summary>
    [Required(ErrorMessage = "角色不能为空")]
    public string Role { get; set; } = string.Empty;

    /// <summary>
    /// 微信OpenId（可选）
    /// </summary>
    [MaxLength(100)]
    public string? OpenId { get; set; }
}
