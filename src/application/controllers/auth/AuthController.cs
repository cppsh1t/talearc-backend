using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using talearc_backend.src.data;
using talearc_backend.src.data.dto;
using talearc_backend.src.data.extensions;
using talearc_backend.src.structure;

namespace talearc_backend.src.application.controllers.auth;

/// <summary>
/// 登录表单
/// </summary>
public class LoginForm
{
    /// <summary>
    /// 用户名
    /// </summary>
    /// <example>张三</example>
    [Required(ErrorMessage = "用户名是必填的")]
    public required string Name { get; set; }
    
    /// <summary>
    /// 密码
    /// </summary>
    /// <example>password123</example>
    [Required(ErrorMessage = "密码是必填的")]
    public required string Password { get; set; }
}

/// <summary>
/// 认证控制器
/// </summary>
[ApiController]
[Route("talearc/api/[controller]")]
public class AuthController(AppDbContext context, ILogger<AuthController> logger) : ControllerBase
{
    private readonly AppDbContext _context = context;
    private readonly ILogger<AuthController> _logger = logger;

    /// <summary>
    /// 用户登录
    /// </summary>
    /// <param name="loginForm">登录表单</param>
    /// <returns>登录结果</returns>
    /// <response code="200">登录成功，返回用户信息</response>
    /// <response code="401">登录失败，用户名或密码错误</response>
    [HttpPost("login")]
    [ProducesResponseType(typeof(ApiResponse<UserDto>), 200)]
    [ProducesResponseType(typeof(ApiResponse<object>), 401)]
    public async Task<IActionResult> Login([FromBody] LoginForm loginForm)
    {
        _logger.LogInformation("用户登录尝试: {Name}", loginForm.Name);
        
        try
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Name == loginForm.Name && u.Password == loginForm.Password);
            
            if (user == null)
            {
                _logger.LogWarning("登录失败: 用户名或密码错误 - {Name}", loginForm.Name);
                var errorResponse = ApiResponse.Fail(401, "登录信息错误");
                return Unauthorized(errorResponse);
            }
            
            var userDto = user.ToDto();
            
            _logger.LogInformation("用户登录成功: {Name}", loginForm.Name);
            var response = ApiResponse.Success(userDto, "登录成功");
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "登录过程中发生错误");
            throw;
        }
    }
}