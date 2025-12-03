using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace talearc_backend.src.data.entities;

public class Misc
{
    /// <summary>
    /// 杂项ID
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
    /// 杂项名称
    /// </summary>
    [Column("name")]
    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;
    
    /// <summary>
    /// 杂项描述
    /// </summary>
    [Column("description")]
    [StringLength(500)]
    public string Description { get; set; } = string.Empty;
    
    /// <summary>
    /// 杂项类型
    /// </summary>
    /// <remarks>例如：道具、地点、事件等</remarks>
    [Column("type")]
    [Required]
    public string Type { get; set; } = string.Empty;

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