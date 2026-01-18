namespace Intchain.UserService.Services;

/// <summary>
/// 微信API服务接口
/// </summary>
public interface IWeChatService
{
    /// <summary>
    /// 通过code换取微信session信息
    /// </summary>
    /// <param name="code">微信登录凭证code</param>
    /// <returns>微信session信息，失败返回null</returns>
    Task<DTOs.WeChatSessionResponse?> GetSessionByCodeAsync(string code);
}
