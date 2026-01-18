namespace Intchain.UserService.DTOs;

/// <summary>
/// 认证响应DTO
/// </summary>
public class AuthResponse
{
    /// <summary>
    /// JWT令牌
    /// </summary>
    public string Token { get; set; } = string.Empty;

    /// <summary>
    /// 用户ID
    /// </summary>
    public int UserId { get; set; }

    /// <summary>
    /// 用户名
    /// </summary>
    public string Username { get; set; } = string.Empty;

    /// <summary>
    /// 角色
    /// </summary>
    public string Role { get; set; } = string.Empty;

    /// <summary>
    /// 实体ID（关联到具体角色表的ID）
    /// </summary>
    public int? EntityId { get; set; }
}
