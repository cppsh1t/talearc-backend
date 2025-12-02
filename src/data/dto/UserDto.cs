namespace talearc_backend.src.data.dto;

/// <summary>
/// 用户数据传输对象
/// </summary>
public class UserDto
{
    /// <summary>
    /// 用户ID
    /// </summary>
    public int Id { get; set; }
    
    /// <summary>
    /// 用户名
    /// </summary>
    public string Name { get; set; } = string.Empty;
    
    /// <summary>
    /// 创建时间
    /// </summary>
    public DateTime CreateAt { get; set; }
}