namespace talearc_backend.src.structure;

/// <summary>
/// IQueryable 扩展方法
/// </summary>
public static class QueryableExtensions
{
    /// <summary>
    /// 分页查询
    /// </summary>
    public static IQueryable<T> Page<T>(this IQueryable<T> query, int page, int size)
    {
        return query.Skip((page - 1) * size).Take(size);
    }

    /// <summary>
    /// 分页查询（使用 PagedRequest）
    /// </summary>
    public static IQueryable<T> Page<T>(this IQueryable<T> query, PagedRequest request)
    {
        return query.Skip((request.Page - 1) * request.Size).Take(request.Size);
    }
}
