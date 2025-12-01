namespace talearc_backend.src.structure;

// 内部数据类
public class ApiResponse<T>
{
    public int Code { get; set; }
    public string Message { get; set; }
    public T? Data { get; set; }

    public ApiResponse(int code, string message, T? data)
    {
        Code = code;
        Message = message;
        Data = data;
    }
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
