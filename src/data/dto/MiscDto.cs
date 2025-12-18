namespace talearc_backend.src.data.dto;

/// <summary>
/// 创建杂项请求
/// </summary>
public class CreateMiscRequest
{
    public int WorldViewId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
}

/// <summary>
/// 更新杂项请求
/// </summary>
public class UpdateMiscRequest
{
    public string? Name { get; set; }
    public string? Description { get; set; }
    public string? Type { get; set; }
}

/// <summary>
/// 杂项响应
/// </summary>
public class MiscResponse
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int WorldViewId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
