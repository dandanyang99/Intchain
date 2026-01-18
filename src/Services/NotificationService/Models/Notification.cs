using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Intchain.NotificationService.Models;

/// <summary>
/// 通知实体
/// </summary>
[Table("notifications")]
public class Notification
{
    /// <summary>
    /// 通知ID
    /// </summary>
    [Key]
    [Column("id")]
    public int Id { get; set; }

    /// <summary>
    /// 接收用户ID
    /// </summary>
    [Required]
    [Column("user_id")]
    public int UserId { get; set; }

    /// <summary>
    /// 通知类型: WeChat, SMS, InApp
    /// </summary>
    [Required]
    [MaxLength(20)]
    [Column("type")]
    public string Type { get; set; } = string.Empty;

    /// <summary>
    /// 通知标题
    /// </summary>
    [Required]
    [MaxLength(100)]
    [Column("title")]
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// 通知内容
    /// </summary>
    [Required]
    [MaxLength(1000)]
    [Column("content")]
    public string Content { get; set; } = string.Empty;

    /// <summary>
    /// 通知状态: Pending, Sent, Failed
    /// </summary>
    [Required]
    [MaxLength(20)]
    [Column("status")]
    public string Status { get; set; } = "Pending";

    /// <summary>
    /// 是否已读
    /// </summary>
    [Column("is_read")]
    public bool IsRead { get; set; } = false;

    /// <summary>
    /// 发送时间
    /// </summary>
    [Column("sent_at")]
    public DateTime? SentAt { get; set; }

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
