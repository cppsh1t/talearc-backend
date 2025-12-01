using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using talearc_backend.src.data;
using talearc_backend.src.data.entities;
using talearc_backend.src.structure;

namespace talearc_backend.src.application.controllers.test;

/// <summary>
/// 人员管理控制器
/// </summary>
[ApiController]
[Route("talearc/api/[controller]")]
public class PersonController(AppDbContext context, ILogger<PersonController> logger) : ControllerBase
{
    private readonly AppDbContext _context = context;
    private readonly ILogger<PersonController> _logger = logger;

    /// <summary>
    /// 获取所有人员列表
    /// </summary>
    /// <returns>人员列表</returns>
    /// <response code="200">成功返回人员列表</response>
    /// <response code="500">服务器内部错误</response>
    [HttpGet("list")]
    [ProducesResponseType(typeof(ApiResponse<List<Person>>), 200)]
    [ProducesResponseType(typeof(ApiResponse<object>), 500)]
    public async Task<IActionResult> GetPersons()
    {
        _logger.LogInformation("开始获取人员列表");
        
        try
        {
            var persons = await _context.Person.ToListAsync();
            _logger.LogInformation("成功获取 {Count} 个人员记录", persons.Count);
            
            var response = ApiResponse.Success(persons);
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取人员列表时发生错误");
            throw;
        }
    }
}