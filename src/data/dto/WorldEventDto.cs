using talearc_backend.src.structure;

namespace talearc_backend.src.data.dto;

/// <summary>
/// 创建世界事件请求
/// </summary>
public class CreateWorldEventRequest
{
    /// <summary>
    /// 世界观ID
    /// </summary>
    public int WorldViewId { get; set; }
    
    /// <summary>
    /// 世界事件名称
    /// </summary>
    public string Name { get; set; } = string.Empty;
    
    /// <summary>
    /// 世界事件描述
    /// </summary>
    public string Description { get; set; } = string.Empty;
    
    /// <summary>
    /// 事件发生时间
    /// </summary>
    public DateTime HappenedAt { get; set; }
    
    /// <summary>
    /// 事件结束时间
    /// </summary>
    public DateTime EndAt { get; set; }
    
    /// <summary>
    /// 关联角色快照ID数组
    /// </summary>
    public int[] RelatedCharacterSnapshotIds { get; set; } = [];
}

/// <summary>
/// 更新世界事件请求
/// </summary>
public class UpdateWorldEventRequest
{
    /// <summary>
    /// 世界事件名称
    /// </summary>
    public string? Name { get; set; }
    
    /// <summary>
    /// 世界事件描述
    /// </summary>
    public string? Description { get; set; }
    
    /// <summary>
    /// 事件发生时间
    /// </summary>
    public DateTime? HappenedAt { get; set; }
    
    /// <summary>
    /// 事件结束时间
    /// </summary>
    public DateTime? EndAt { get; set; }
    
    /// <summary>
    /// 关联角色快照ID数组
    /// </summary>
    public int[]? RelatedCharacterSnapshotIds { get; set; }
}

/// <summary>
/// 世界事件响应
/// </summary>
public class WorldEventResponse
{
    /// <summary>
    /// 世界事件ID
    /// </summary>
    public int Id { get; set; }
    
    /// <summary>
    /// 用户ID
    /// </summary>
    public int UserId { get; set; }
    
    /// <summary>
    /// 世界观ID
    /// </summary>
    public int WorldViewId { get; set; }
    
    /// <summary>
    /// 世界事件名称
    /// </summary>
    public string Name { get; set; } = string.Empty;
    
    /// <summary>
    /// 世界事件描述
    /// </summary>
    public string Description { get; set; } = string.Empty;
    
    /// <summary>
    /// 事件发生时间
    /// </summary>
    public DateTime HappenedAt { get; set; }
    
    /// <summary>
    /// 事件结束时间
    /// </summary>
    public DateTime EndAt { get; set; }
    
    /// <summary>
    /// 关联角色快照ID数组
    /// </summary>
    public int[] RelatedCharacterSnapshotIds { get; set; } = [];
    
    /// <summary>
    /// 创建时间
    /// </summary>
    public DateTime CreatedAt { get; set; }
    
    /// <summary>
    /// 更新时间
    /// </summary>
    public DateTime UpdatedAt { get; set; }
}

/// <summary>
/// 世界事件查询请求（分页+查询条件）
/// </summary>
public class WorldEventPagedRequest : PagedRequest
{
    /// <summary>
    /// 世界观ID（可选）
    /// </summary>
    public int? WorldViewId { get; set; }
}
