using System.Diagnostics;

namespace talearc_backend.src.middleware;

public class RequestLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestLoggingMiddleware> _logger;

    public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var stopwatch = Stopwatch.StartNew();
        
        _logger.LogInformation("请求开始: {Method} {Path} {QueryString}", 
            context.Request.Method, 
            context.Request.Path, 
            context.Request.QueryString);

        try
        {
            await _next(context);
        }
        finally
        {
            stopwatch.Stop();
            _logger.LogInformation("请求完成: {Method} {Path} {StatusCode} 耗时 {ElapsedMs}ms", 
                context.Request.Method, 
                context.Request.Path, 
                context.Response.StatusCode, 
                stopwatch.ElapsedMilliseconds);
        }
    }
}