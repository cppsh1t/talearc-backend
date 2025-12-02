namespace talearc_backend.src.data.dto;

/// <summary>
/// 登录响应数据传输对象
/// </summary>
public class LoginResponseDto
{
    /// <summary>
    /// JWT Token
    /// </summary>
    public string Token { get; set; } = string.Empty;
    
    /// <summary>
    /// 用户信息
    /// </summary>
    public UserDto User { get; set; } = new();
}