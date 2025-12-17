namespace talearc_backend.src.data.dto;

/// <summary>
/// 章节请求DTO
/// </summary>
public class ChapterRequest
{
    public string Title { get; set; } = string.Empty;
    public string Summary { get; set; } = string.Empty;
    public int Order { get; set; }
    public string Content { get; set; } = string.Empty;
    public int[] ReferencedSnapshotIds { get; set; } = [];
    public int[] ReferencedEventIds { get; set; } = [];
    public int[] ReferencedMiscIds { get; set; } = [];
}

/// <summary>
/// 章节响应DTO
/// </summary>
public class ChapterResponse
{
    public int Id { get; set; }
    public string Uuid { get; set; } = string.Empty;
    public int NovelId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Summary { get; set; } = string.Empty;
    public int Order { get; set; }
    public int[] ReferencedSnapshotIds { get; set; } = [];
    public int[] ReferencedEventIds { get; set; } = [];
    public int[] ReferencedMiscIds { get; set; } = [];
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public string? Content { get; set; }
}
