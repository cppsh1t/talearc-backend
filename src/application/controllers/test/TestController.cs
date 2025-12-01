using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using talearc_backend.src.structure;

namespace talearc_backend.src.application.controllers.test;

/// <summary>
/// 问候请求表单
/// </summary>
public class GreetingForm
{
    /// <summary>
    /// 用户姓名
    /// </summary>
    /// <example>张三</example>
    [Required(ErrorMessage = "姓名是必填的。")]
    [StringLength(50, ErrorMessage = "姓名长度不能超过50个字符。")]
    public required string Name { get; set; }
    
    /// <summary>
    /// 用户年龄
    /// </summary>
    /// <example>25</example>
    [Range(1, 150, ErrorMessage = "年龄必须在1到150岁之间。")]
    public int Age { get; set; }
}

[ApiController]
[Route("talearc/api/[controller]")]
public class TestController(ILogger<TestController> logger) : ControllerBase
{
    private readonly ILogger<TestController> _logger = logger;
    /// <summary>
    /// 发送问候消息
    /// </summary>
    /// <param name="request">问候请求信息</param>
    /// <returns>包含问候消息的响应</returns>
    /// <response code="200">成功返回问候消息</response>
    /// <response code="400">请求参数验证失败</response>
    [HttpPost("greet")] 
    [ProducesResponseType(typeof(ApiResponse<string>), 200)]
    [ProducesResponseType(typeof(ApiResponse<object>), 400)]
    public IActionResult Greet([FromBody] GreetingForm request)
    {
        _logger.LogInformation("收到问候请求: 姓名={Name}, 年龄={Age}", request.Name, request.Age);

        string responseMessage = $"你好，{request.Name}！你今年 {request.Age} 岁了。你的请求已成功处理。";
 
        var response = ApiResponse.Success(responseMessage);
        _logger.LogInformation("问候请求处理成功");
        
        return Ok(response); 
    }
    /// <summary>
    /// 模拟服务器错误测试
    /// </summary>
    /// <returns>模拟的服务器内部错误响应</returns>
    /// <response code="500">服务器内部错误</response>
    [HttpGet("fail-test")]
    [ProducesResponseType(typeof(ApiResponse<object>), 500)]
    public IActionResult FailTest()
    {
        _logger.LogWarning("模拟失败测试被调用");
  
        var errorResponse = ApiResponse.Fail(500, "这是一个模拟的服务器内部错误。");

        return StatusCode(500, errorResponse);
    }
}
