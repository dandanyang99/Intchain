using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Intchain.UserService.Models;

/// <summary>
/// 用户实体
/// </summary>
[Table("users")]
public class User
{
    /// <summary>
    /// 用户ID
    /// </summary>
    [Key]
    [Column("id")]
    public int Id { get; set; }

    /// <summary>
    /// 用户名
    /// </summary>
    [Required]
    [MaxLength(50)]
    [Column("username")]
    public string Username { get; set; } = string.Empty;

    /// <summary>
    /// 密码哈希
    /// </summary>
    [Required]
    [MaxLength(255)]
    [Column("password_hash")]
    public string PasswordHash { get; set; } = string.Empty;

    /// <summary>
    /// 微信OpenId
    /// </summary>
    [MaxLength(100)]
    [Column("openid")]
    public string? OpenId { get; set; }

    /// <summary>
    /// 微信会话密钥（用于后续解密用户敏感数据）
    /// </summary>
    [MaxLength(255)]
    [Column("session_key")]
    public string? SessionKey { get; set; }

    /// <summary>
    /// 微信UnionId（用于跨应用用户识别）
    /// </summary>
    [MaxLength(100)]
    [Column("unionid")]
    public string? UnionId { get; set; }

    /// <summary>
    /// OpenId绑定时间
    /// </summary>
    [Column("openid_bound_at")]
    public DateTime? OpenIdBoundAt { get; set; }

    /// <summary>
    /// 用户角色: Admin, LotteryCenter, PrintingFactory, SalesOutlet
    /// </summary>
    [Required]
    [MaxLength(20)]
    [Column("role")]
    public string Role { get; set; } = string.Empty;

    /// <summary>
    /// 关联实体ID（根据角色关联到不同表）
    /// </summary>
    [Column("entity_id")]
    public int? EntityId { get; set; }

    /// <summary>
    /// 是否激活
    /// </summary>
    [Column("is_active")]
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// 创建时间
    /// </summary>
    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// 更新时间
    /// </summary>
    [Column("updated_at")]
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
