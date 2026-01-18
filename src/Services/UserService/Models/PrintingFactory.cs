using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Intchain.UserService.Models;

/// <summary>
/// 印刷厂实体
/// </summary>
[Table("printing_factories")]
public class PrintingFactory
{
    /// <summary>
    /// 印刷厂ID
    /// </summary>
    [Key]
    [Column("id")]
    public int Id { get; set; }

    /// <summary>
    /// 印刷厂名称
    /// </summary>
    [Required]
    [MaxLength(100)]
    [Column("name")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 联系人
    /// </summary>
    [Required]
    [MaxLength(50)]
    [Column("contact_person")]
    public string ContactPerson { get; set; } = string.Empty;

    /// <summary>
    /// 联系电话
    /// </summary>
    [Required]
    [MaxLength(20)]
    [Column("contact_phone")]
    public string ContactPhone { get; set; } = string.Empty;

    /// <summary>
    /// 地址
    /// </summary>
    [Required]
    [MaxLength(200)]
    [Column("address")]
    public string Address { get; set; } = string.Empty;

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
