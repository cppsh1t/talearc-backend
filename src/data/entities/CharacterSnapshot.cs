using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace talearc_backend.src.data.entities;

/// <summary>
/// 角色快照实体
/// </summary>
[Table("character_snapshots")]
public class CharacterSnapshot
{
    /// <summary>
    /// 快照ID
    /// </summary>
    [Key]
    [Column("id")]
    public int Id { get; set; }
    
    /// <summary>
    /// 用户ID
    /// </summary>
    [Column("user_id")]
    [Required]
    public int UserId { get; set; }
    
    /// <summary>
    /// 世界观ID
    /// </summary>
    [Column("world_view_id")]
    [Required]
    public int WorldViewId { get; set; }
    
    /// <summary>
    /// 角色ID
    /// </summary>
    [Column("character_id")]
    [Required]
    public int CharacterId { get; set; }
    
    /// <summary>
    /// 快照名称
    /// </summary>
    [Column("name")]
    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;
    
    /// <summary>
    /// 快照描述
    /// </summary>
    [Column("description")]
    [StringLength(500)]
    public string Description { get; set; } = string.Empty;
    
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
    
    /// <summary>
    /// 备注
    /// </summary>
    [Column("note")]
    [StringLength(500)]
    public string Note { get; set; } = string.Empty;
}