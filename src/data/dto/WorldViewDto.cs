using talearc_backend.src.structure;

namespace talearc_backend.src.data.dto;

/// <summary>
/// 创建世界观请求
/// </summary>
public class CreateWorldViewRequest
{
    /// <summary>
    /// 世界观名称
    /// </summary>
    public string Name { get; set; } = string.Empty;
    
    /// <summary>
    /// 世界观描述
    /// </summary>
    public string Description { get; set; } = string.Empty;
    
    /// <summary>
    /// 备注
    /// </summary>
    public string Notes { get; set; } = string.Empty;
}

/// <summary>
/// 更新世界观请求
/// </summary>
public class UpdateWorldViewRequest
{
    /// <summary>
    /// 世界观名称
    /// </summary>
    public string? Name { get; set; }
    
    /// <summary>
    /// 世界观描述
    /// </summary>
    public string? Description { get; set; }
    
    /// <summary>
    /// 备注
    /// </summary>
    public string? Notes { get; set; }
}

/// <summary>
/// 世界观响应
/// </summary>
public class WorldViewResponse
{
    /// <summary>
    /// 世界观ID
    /// </summary>
    public int Id { get; set; }
    
    /// <summary>
    /// 用户ID
    /// </summary>
    public int UserId { get; set; }
    
    /// <summary>
    /// 角色ID数组
    /// </summary>
    public int[] CharacterIds { get; set; } = [];
    
    /// <summary>
    /// 世界杂项ID数组
    /// </summary>
    public int[] MiscIds { get; set; } = [];
    
    /// <summary>
    /// 世界事件ID数组
    /// </summary>
    public int[] WorldEventIds { get; set; } = [];
    
    /// <summary>
    /// 世界观名称
    /// </summary>
    public string Name { get; set; } = string.Empty;
    
    /// <summary>
    /// 世界观描述
    /// </summary>
    public string Description { get; set; } = string.Empty;
    
    /// <summary>
    /// 备注
    /// </summary>
    public string Notes { get; set; } = string.Empty;
    
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
/// 世界观查询请求（分页）
/// </summary>
public class WorldViewPagedRequest : PagedRequest
{
}
