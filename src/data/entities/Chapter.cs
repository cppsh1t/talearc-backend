using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace talearc_backend.src.data.entities;

/// <summary>
/// 章节实体
/// </summary>
[Table("chapters")]
public class Chapter
{
    /// <summary>
    /// 章节ID
    /// </summary>
    [Key]
    [Column("id")]
    public int Id { get; set; }
    
    /// <summary>
    /// 章节UUID（用于文件路径定位）
    /// </summary>
    [Column("uuid")]
    [Required]
    [StringLength(36)]
    public string Uuid { get; set; } = Guid.NewGuid().ToString();
    
    /// <summary>
    /// 小说ID
    /// </summary>
    [Column("novel_id")]
    [Required]
    public int NovelId { get; set; }
    
    /// <summary>
    /// 章节标题
    /// </summary>
    [Column("title")]
    [Required]
    [StringLength(200)]
    public string Title { get; set; } = string.Empty;
    
    /// <summary>
    /// 章节概述
    /// </summary>
    [Column("summary")]
    [StringLength(1000)]
    public string Summary { get; set; } = string.Empty;
    
    /// <summary>
    /// 章节排序
    /// </summary>
    [Column("order")]
    [Required]
    public int Order { get; set; }
    
    /// <summary>
    /// 引用的角色快照ID数组
    /// </summary>
    [Column("referenced_snapshot_ids")]
    public int[] ReferencedSnapshotIds { get; set; } = [];
    
    /// <summary>
    /// 引用的事件ID数组
    /// </summary>
    [Column("referenced_event_ids")]
    public int[] ReferencedEventIds { get; set; } = [];
    
    /// <summary>
    /// 引用的杂项ID数组
    /// </summary>
    [Column("referenced_misc_ids")]
    public int[] ReferencedMiscIds { get; set; } = [];
    
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
