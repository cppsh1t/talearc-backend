using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace talearc_backend.src.data.entities;

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
    [Column("name")]
    [Required]
    [StringLength(50)]
    public string Name { get; set; } = string.Empty;
    
    /// <summary>
    /// 密码
    /// </summary>
    [Column("password")]
    [Required]
    [StringLength(255)]
    public string Password { get; set; } = string.Empty;
    
    /// <summary>
    /// 创建时间
    /// </summary>
    [Column("create_at")]
    public DateTime CreateAt { get; set; } = DateTime.UtcNow;
}