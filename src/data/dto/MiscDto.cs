namespace talearc_backend.src.data.dto;

/// <summary>
/// 创建杂项请求
/// </summary>
public class CreateMiscRequest
{
    /// <summary>
    /// 世界观ID
    /// </summary>
    public int WorldViewId { get; set; }
    
    /// <summary>
    /// 杂项名称
    /// </summary>
    public string Name { get; set; } = string.Empty;
    
    /// <summary>
    /// 杂项描述
    /// </summary>
    public string Description { get; set; } = string.Empty;
    
    /// <summary>
    /// 杂项类型（例如：道具、地点、事件等）
    /// </summary>
    public string Type { get; set; } = string.Empty;
}

/// <summary>
/// 更新杂项请求
/// </summary>
public class UpdateMiscRequest
{
    /// <summary>
    /// 杂项名称
    /// </summary>
    public string? Name { get; set; }
    
    /// <summary>
    /// 杂项描述
    /// </summary>
    public string? Description { get; set; }
    
    /// <summary>
    /// 杂项类型
    /// </summary>
    public string? Type { get; set; }
}

/// <summary>
/// 杂项响应
/// </summary>
public class MiscResponse
{
    /// <summary>
    /// 杂项ID
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
    /// 杂项名称
    /// </summary>
    public string Name { get; set; } = string.Empty;
    
    /// <summary>
    /// 杂项描述
    /// </summary>
    public string Description { get; set; } = string.Empty;
    
    /// <summary>
    /// 杂项类型
    /// </summary>
    public string Type { get; set; } = string.Empty;
    
    /// <summary>
    /// 创建时间
    /// </summary>
    public DateTime CreatedAt { get; set; }
    
    /// <summary>
    /// 更新时间
    /// </summary>
    public DateTime UpdatedAt { get; set; }
}
