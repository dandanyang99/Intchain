namespace Intchain.UserService.DTOs;

/// <summary>
/// 获取OpenId响应DTO
/// </summary>
public class GetOpenIdResponse
{
    /// <summary>
    /// 微信OpenId
    /// </summary>
    public string OpenId { get; set; } = string.Empty;

    /// <summary>
    /// 微信UnionId（可选）
    /// </summary>
    public string? UnionId { get; set; }

    /// <summary>
    /// 是否已被其他用户绑定
    /// </summary>
    public bool IsAlreadyBound { get; set; }
}
