using System.Text.Json.Serialization;

namespace Intchain.UserService.DTOs;

/// <summary>
/// 微信session响应DTO（来自微信API）
/// </summary>
public class WeChatSessionResponse
{
    /// <summary>
    /// 用户唯一标识
    /// </summary>
    [JsonPropertyName("openid")]
    public string OpenId { get; set; } = string.Empty;

    /// <summary>
    /// 会话密钥
    /// </summary>
    [JsonPropertyName("session_key")]
    public string SessionKey { get; set; } = string.Empty;

    /// <summary>
    /// 用户在开放平台的唯一标识符（可选）
    /// </summary>
    [JsonPropertyName("unionid")]
    public string? UnionId { get; set; }

    /// <summary>
    /// 错误码（微信API返回）
    /// </summary>
    [JsonPropertyName("errcode")]
    public int? ErrCode { get; set; }

    /// <summary>
    /// 错误信息（微信API返回）
    /// </summary>
    [JsonPropertyName("errmsg")]
    public string? ErrMsg { get; set; }
}
