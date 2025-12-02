using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using talearc_backend.src.data;
using talearc_backend.src.data.dto;
using talearc_backend.src.data.extensions;
using talearc_backend.src.structure;
using talearc_backend.src.application.service;

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
public class AuthController(AppDbContext context, ILogger<AuthController> logger, JwtTokenGenerator tokenGenerator) : ControllerBase
{
    private readonly AppDbContext _context = context;
    private readonly ILogger<AuthController> _logger = logger;
    private readonly JwtTokenGenerator _tokenGenerator = tokenGenerator;

    /// <summary>
    /// 用户登录
    /// </summary>
    /// <param name="loginForm">登录表单</param>
    /// <returns>登录结果</returns>
    /// <response code="200">登录成功，返回 Token 和用户信息</response>
    /// <response code="401">登录失败，用户名或密码错误</response>
    [HttpPost("login")]
    [ProducesResponseType(typeof(ApiResponse<LoginResponseDto>), 200)]
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
            var token = _tokenGenerator.GenerateToken(user.Id, user.Name);
            
            var loginResponse = new LoginResponseDto
            {
                Token = token,
                User = userDto
            };
            
            _logger.LogInformation("用户登录成功: {Name}", loginForm.Name);
            var response = ApiResponse.Success(loginResponse, "登录成功");
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "登录过程中发生错误");
            throw;
        }
    }
    
    /// <summary>
    /// 获取当前用户信息
    /// </summary>
    /// <returns>用户信息</returns>
    /// <response code="200">成功返回用户信息</response>
    /// <response code="401">未授权</response>
    [Authorize]
    [HttpGet("userinfo")]
    [ProducesResponseType(typeof(ApiResponse<UserDto>), 200)]
    [ProducesResponseType(typeof(ApiResponse<object>), 401)]
    public async Task<IActionResult> GetUserInfo()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        
        if (!int.TryParse(userIdClaim, out var userId))
        {
            _logger.LogWarning("无效的用户ID");
            return Unauthorized(ApiResponse.Fail(401, "无效的用户信息"));
        }
        
        var user = await _context.Users.FindAsync(userId);
        if (user == null)
        {
            _logger.LogWarning("用户不存在: {UserId}", userId);
            return Unauthorized(ApiResponse.Fail(401, "用户不存在"));
        }
        
        var userDto = user.ToDto();
        _logger.LogInformation("获取用户信息: {Name}", user.Name);
        var response = ApiResponse.Success(userDto, "获取成功");
        return Ok(response);
    }
    
    /// <summary>
    /// 用户登出
    /// </summary>
    /// <returns>登出结果</returns>
    /// <response code="200">登出成功</response>
    /// <response code="401">未授权</response>
    [Authorize]
    [HttpPost("logout")]
    [ProducesResponseType(typeof(ApiResponse<object>), 200)]
    [ProducesResponseType(typeof(ApiResponse<object>), 401)]
    public IActionResult Logout()
    {
        var userName = User.Identity?.Name;
        _logger.LogInformation("用户登出: {Name}", userName);
        var response = ApiResponse.Success<object>(null!, "登出成功");
        return Ok(response);
    }
}