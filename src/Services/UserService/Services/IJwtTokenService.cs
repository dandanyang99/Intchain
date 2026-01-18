using Intchain.UserService.Models;

namespace Intchain.UserService.Services;

/// <summary>
/// JWT令牌服务接口
/// </summary>
public interface IJwtTokenService
{
    /// <summary>
    /// 生成JWT令牌
    /// </summary>
    string GenerateToken(User user);

    /// <summary>
    /// 验证JWT令牌
    /// </summary>
    bool ValidateToken(string token);
}
