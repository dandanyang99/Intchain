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
}
