using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace talearc_backend.src.data.entities;

/// <summary>
/// 注册密钥实体
/// </summary>
[Table("registration_keys")]
public class RegistrationKey
{
    /// <summary>
    /// 密钥值
    /// </summary>
    [Key]
    [Column("key")]
    [StringLength(255)]
    public string Key { get; set; } = string.Empty;
    
    /// <summary>
    /// 使用该密钥的用户ID
    /// </summary>
    [Column("user_id")]
    public int? UserId { get; set; }
    
    /// <summary>
    /// 使用时间
    /// </summary>
    [Column("used_at")]
    public DateTime? UsedAt { get; set; }
}