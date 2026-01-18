using Intchain.UserService.DTOs;

namespace Intchain.UserService.Services;

/// <summary>
/// 认证服务接口
/// </summary>
public interface IAuthService
{
    /// <summary>
    /// 用户注册
    /// </summary>
    Task<AuthResponse?> RegisterAsync(RegisterRequest request);

    /// <summary>
    /// 用户登录
    /// </summary>
    Task<AuthResponse?> LoginAsync(LoginRequest request);

    /// <summary>
    /// 微信登录
    /// </summary>
    Task<AuthResponse?> WeChatLoginAsync(WeChatLoginRequest request);

    /// <summary>
    /// 绑定微信账号
    /// </summary>
    Task<bool> BindWeChatAsync(int userId, string code);

    /// <summary>
    /// 获取微信OpenId（用于注册时绑定）
    /// </summary>
    Task<GetOpenIdResponse?> GetOpenIdAsync(string code);
}
