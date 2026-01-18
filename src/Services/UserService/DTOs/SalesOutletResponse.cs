namespace Intchain.UserService.DTOs;

/// <summary>
/// 销售网点响应
/// </summary>
public class SalesOutletResponse
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int LotteryCenterId { get; set; }
    public string ContactPerson { get; set; } = string.Empty;
    public string ContactPhone { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    /// <summary>
    /// 默认用户信息（如果创建时生成了默认用户）
    /// </summary>
    public DefaultUserInfo? DefaultUser { get; set; }
}

/// <summary>
/// 默认用户信息
/// </summary>
public class DefaultUserInfo
{
    public int UserId { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}
