using talearc_backend.src.application.service;

namespace talearc_backend.src.middleware;

public class JwtBlacklistMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<JwtBlacklistMiddleware> _logger;

    public JwtBlacklistMiddleware(RequestDelegate next, ILogger<JwtBlacklistMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context, TokenBlacklistService blacklistService)
    {
        var authHeader = context.Request.Headers["Authorization"].ToString();
        
        if (!string.IsNullOrEmpty(authHeader) && authHeader.StartsWith("Bearer "))
        {
            var token = authHeader.Substring("Bearer ".Length).Trim();
            
            if (await blacklistService.IsTokenBlacklistedAsync(token))
            {
                _logger.LogWarning("尝试使用已登出的 Token");
                context.Response.StatusCode = 401;
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsJsonAsync(new { code = 401, message = "Token 已失效，请重新登录" });
                return;
            }
        }

        await _next(context);
    }
}