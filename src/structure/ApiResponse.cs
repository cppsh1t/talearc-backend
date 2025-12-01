namespace talearc_backend.src.structure;

public class ApiResponse<T>
{
    public int Code { get; private set; }
    public string Message { get; private set; }
    public T? Data { get; private set; }

    // 私有构造函数，防止外部直接 new
    private ApiResponse(int code, string message, T? data)
    {
        Code = code;
        Message = message;
        Data = data;
    }

    // 静态工厂方法：成功
    public static ApiResponse<T> Success(T data, string message = "操作成功")
    {
        return new ApiResponse<T>(200, message, data);
    }

    // 静态工厂方法：失败
    public static ApiResponse<T> Fail(int code, string message)
    {
        // 对于失败情况，Data 通常是默认值 (比如 null)
        return new ApiResponse<T>(code, message, default(T));
    }
}

// 提供一个非泛型的版本，方便在不返回数据时调用 Fail
public static class ApiResponse
{
    public static ApiResponse<object> Fail(int code, string message)
    {
        return ApiResponse<object>.Fail(code, message);
    }
}
