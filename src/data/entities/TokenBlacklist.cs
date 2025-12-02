using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace talearc_backend.src.data.entities;

/// <summary>
/// Token 黑名单实体
/// </summary>
[Table("token_blacklist")]
public class TokenBlacklist
{
    /// <summary>
    /// ID
    /// </summary>
    [Key]
    [Column("id")]
    public int Id { get; set; }
    
    /// <summary>
    /// Token 值
    /// </summary>
    [Column("token")]
    [Required]
    public string Token { get; set; } = string.Empty;
    
    /// <summary>
    /// Token 过期时间
    /// </summary>
    [Column("expires_at")]
    public DateTime ExpiresAt { get; set; }
    
    /// <summary>
    /// 创建时间
    /// </summary>
    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}