namespace talearc_backend.src.structure;

/// <summary>
/// API 响应统一格式
/// </summary>
/// <typeparam name="T">响应数据类型</typeparam>
public class ApiResponse<T>(int code, string message, T? data)
{
    /// <summary>
    /// 响应状态码
    /// </summary>
    public int Code { get; set; } = code;
    
    /// <summary>
    /// 响应消息
    /// </summary>
    public string Message { get; set; } = message;
    
    /// <summary>
    /// 响应数据
    /// </summary>
    public T? Data { get; set; } = data;
}

/// <summary>
/// API 响应工具类
/// </summary>
public static class ApiResponse
{
    public static ApiResponse<T> Success<T>(T data, string message = "操作成功")
    {
        return new ApiResponse<T>(200, message, data);
    }

    public static ApiResponse<object> Fail(int code, string message)
    {
        return new ApiResponse<object>(code, message, null);
    }
}
