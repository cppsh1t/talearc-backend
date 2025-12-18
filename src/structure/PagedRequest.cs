namespace talearc_backend.src.structure;

/// <summary>
/// 分页请求基类
/// </summary>
public class PagedRequest
{
    /// <summary>
    /// 页码（从1开始）
    /// </summary>
    public int Page { get; set; } = 1;

    /// <summary>
    /// 每页大小
    /// </summary>
    public int Size { get; set; } = 10;
}
