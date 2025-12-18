using talearc_backend.src.structure;

namespace talearc_backend.src.data.dto;

/// <summary>
/// 杂项查询请求（分页+查询条件）
/// </summary>
public class MiscPagedRequest : PagedRequest
{
    /// <summary>
    /// 世界观ID（可选）
    /// </summary>
    public int? WorldViewId { get; set; }
}
