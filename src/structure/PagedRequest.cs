namespace talearc_backend.src.structure;

/// <summary>
/// 分页请求参数
/// </summary>
/// <typeparam name="T">查询参数类型</typeparam>
public class PagedRequest<T>
{
    /// <summary>
    /// 页码（从1开始）
    /// </summary>
    public int Page { get; set; } = 1;

    /// <summary>
    /// 每页大小
    /// </summary>
    public int Size { get; set; } = 10;

    /// <summary>
    /// 查询参数
    /// </summary>
    public T? Query { get; set; }
}
