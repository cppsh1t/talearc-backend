namespace talearc_backend.src.data.dto;

/// <summary>
/// 注册请求
/// </summary>
public class RegisterDto
{
    /// <summary>
    /// 用户名
    /// </summary>
    public string Name { get; set; } = string.Empty;
    
    /// <summary>
    /// 密码
    /// </summary>
    public string Password { get; set; } = string.Empty;
    
    /// <summary>
    /// 注册密钥
    /// </summary>
    public string RegistrationKey { get; set; } = string.Empty;
}