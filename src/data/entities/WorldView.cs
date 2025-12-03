using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace talearc_backend.src.data.entities;

public class WorldView
{
    /// <summary>
    /// 世界观ID
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
    /// 角色ID数组
    /// </summary>
    /// <remarks>存储与该世界观相关的角色ID列表</remarks>
    [Column("character_ids")]
    [Required]
    public int[] CharacterIds { get; set; } = [];

    /// <summary>
    /// 世界杂项ID数组
    /// </summary>
    /// <remarks>存储与该世界观相关的杂项ID列表</remarks>
    [Column("misc_ids")]
    [Required]
    public int[] MiscIds { get; set; } = [];

    /// <summary>
    /// 世界事件ID数组
    /// </summary>
    /// <remarks>存储与该世界观相关的世界事件ID列表</remarks>
    [Column("world_event_ids")]
    [Required]
    public int[] WorldEventIds { get; set; } = [];

    /// <summary>
    /// 世界观名称
    /// </summary>
    [Column("name")]
    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;
    
    /// <summary>
    /// 世界观描述
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
    [Column("notes")]
    public string Notes { get; set; } = string.Empty;
}