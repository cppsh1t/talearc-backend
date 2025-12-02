using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using talearc_backend.src.data;
using talearc_backend.src.data.entities;
using talearc_backend.src.structure;

namespace talearc_backend.src.application.controllers.test;

/// <summary>
/// 用户管理控制器
/// </summary>
[ApiController]
[Route("talearc/api/[controller]")]
public class UserController(AppDbContext context, ILogger<UserController> logger) : ControllerBase
{
    private readonly AppDbContext _context = context;
    private readonly ILogger<UserController> _logger = logger;

    /// <summary>
    /// 获取所有用户列表
    /// </summary>
    /// <returns>用户列表</returns>
    /// <response code="200">成功返回用户列表</response>
    /// <response code="500">服务器内部错误</response>
    [HttpGet("list")]
    [ProducesResponseType(typeof(ApiResponse<List<User>>), 200)]
    [ProducesResponseType(typeof(ApiResponse<object>), 500)]
    public async Task<IActionResult> GetUsers()
    {
        _logger.LogInformation("开始获取用户列表");
        
        try
        {
            var users = await _context.Users.ToListAsync();
            _logger.LogInformation("成功获取 {Count} 个用户记录", users.Count);
            
            var response = ApiResponse.Success(users);
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取用户列表时发生错误");
            throw;
        }
    }
}