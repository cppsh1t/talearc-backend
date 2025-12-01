namespace talearc_backend.src.structure;

// 内部数据类
public class ApiResponse<T>(int code, string message, T? data)
{
    public int Code { get; set; } = code;
    public string Message { get; set; } = message;
    public T? Data { get; set; } = data;
}

// 外部工具类，利用泛型推断
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
