using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace talearc_backend.src.data.entities;

public class WorldEvent
{
    /// <summary>
    /// 世界事件ID
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
    /// 世界事件名称
    /// </summary>
    [Column("name")]
    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;
    
    /// <summary>
    /// 世界事件描述
    /// </summary>
    [Column("description")]
    [StringLength(500)]
    public string Description { get; set; } = string.Empty;
    
    /// <summary>
    /// 事件发生时间
    /// </summary>
    /// <remarks>表示事件实际发生的时间点</remarks>
    [Column("happened_at")]
    [Required]
    public DateTime HappenedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// 事件结束时间
    /// </summary>
    /// <remarks>表示事件结束的时间点</remarks>
    [Column("end_at")]
    [Required]
    public DateTime EndAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// 所属世界观ID
    /// </summary>
    /// <remarks>关联的世界观实体ID</remarks>
    [Column("world_view_id")]
    [Required]
    public int WorldViewId { get; set; }

    /// <summary>
    /// 创建时间
    /// </summary>
    [Column("created_at")]
    [Required]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// 关联角色快照ID数组
    /// </summary>
    /// <remarks>存储与该事件相关的角色快照ID列表</remarks>
    [Column("related_character_snapshot_ids")]
    [Required]
    public int[] RelatedCharacterSnapshotIds { get; set; } = [];
    
    /// <summary>
    /// 更新时间
    /// </summary>
    [Column("updated_at")]
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}