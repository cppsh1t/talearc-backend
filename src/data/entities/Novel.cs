using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace talearc_backend.src.data.entities;

/// <summary>
/// 小说实体
/// </summary>
[Table("novels")]
public class Novel
{
    /// <summary>
    /// 小说ID
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
    /// 小说标题
    /// </summary>
    [Column("title")]
    [Required]
    [StringLength(200)]
    public string Title { get; set; } = string.Empty;
    
    /// <summary>
    /// 小说简介
    /// </summary>
    [Column("description")]
    [StringLength(1000)]
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
}
