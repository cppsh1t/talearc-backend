using talearc_backend.src.structure;

namespace talearc_backend.src.data.dto;

/// <summary>
/// 创建角色请求
/// </summary>
public class CreateCharacterRequest
{
    /// <summary>
    /// 世界观ID
    /// </summary>
    public int WorldViewId { get; set; }
    
    /// <summary>
    /// 角色名称
    /// </summary>
    public string Name { get; set; } = string.Empty;
    
    /// <summary>
    /// 角色描述
    /// </summary>
    public string Description { get; set; } = string.Empty;
    
    /// <summary>
    /// 备注
    /// </summary>
    public string Note { get; set; } = string.Empty;
}

/// <summary>
/// 更新角色请求
/// </summary>
public class UpdateCharacterRequest
{
    /// <summary>
    /// 角色名称
    /// </summary>
    public string? Name { get; set; }
    
    /// <summary>
    /// 角色描述
    /// </summary>
    public string? Description { get; set; }
    
    /// <summary>
    /// 备注
    /// </summary>
    public string? Note { get; set; }
}

/// <summary>
/// 角色响应
/// </summary>
public class CharacterResponse
{
    /// <summary>
    /// 角色ID
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
    /// 快照ID数组
    /// </summary>
    public int[] SnapshotIds { get; set; } = [];
    
    /// <summary>
    /// 角色名称
    /// </summary>
    public string Name { get; set; } = string.Empty;
    
    /// <summary>
    /// 角色描述
    /// </summary>
    public string Description { get; set; } = string.Empty;
    
    /// <summary>
    /// 备注
    /// </summary>
    public string Note { get; set; } = string.Empty;
    
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
/// 角色查询请求（分页+查询条件）
/// </summary>
public class CharacterPagedRequest : PagedRequest
{
    /// <summary>
    /// 世界观ID（可选）
    /// </summary>
    public int? WorldViewId { get; set; }
}

/// <summary>
/// 创建角色快照请求
/// </summary>
public class CreateCharacterSnapshotRequest
{
    /// <summary>
    /// 世界观ID
    /// </summary>
    public int WorldViewId { get; set; }
    
    /// <summary>
    /// 角色ID
    /// </summary>
    public int CharacterId { get; set; }
    
    /// <summary>
    /// 快照名称
    /// </summary>
    public string Name { get; set; } = string.Empty;
    
    /// <summary>
    /// 快照描述
    /// </summary>
    public string Description { get; set; } = string.Empty;
    
    /// <summary>
    /// 备注
    /// </summary>
    public string Note { get; set; } = string.Empty;
}

/// <summary>
/// 更新角色快照请求
/// </summary>
public class UpdateCharacterSnapshotRequest
{
    /// <summary>
    /// 快照名称
    /// </summary>
    public string? Name { get; set; }
    
    /// <summary>
    /// 快照描述
    /// </summary>
    public string? Description { get; set; }
    
    /// <summary>
    /// 备注
    /// </summary>
    public string? Note { get; set; }
}

/// <summary>
/// 角色快照响应
/// </summary>
public class CharacterSnapshotResponse
{
    /// <summary>
    /// 快照ID
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
    /// 角色ID
    /// </summary>
    public int CharacterId { get; set; }
    
    /// <summary>
    /// 快照名称
    /// </summary>
    public string Name { get; set; } = string.Empty;
    
    /// <summary>
    /// 快照描述
    /// </summary>
    public string Description { get; set; } = string.Empty;
    
    /// <summary>
    /// 备注
    /// </summary>
    public string Note { get; set; } = string.Empty;
    
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
/// 角色快照查询请求（分页+查询条件）
/// </summary>
public class CharacterSnapshotPagedRequest : PagedRequest
{
    /// <summary>
    /// 世界观ID（可选）
    /// </summary>
    public int? WorldViewId { get; set; }
    
    /// <summary>
    /// 角色ID（可选）
    /// </summary>
    public int? CharacterId { get; set; }
}
