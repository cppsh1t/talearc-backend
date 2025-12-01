using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using talearc_backend.src.structure;

namespace talearc_backend.src.application.controllers.test;

public class GreetingForm
{
    [Required(ErrorMessage = "姓名是必填的。")]
    [StringLength(50, ErrorMessage = "姓名长度不能超过50个字符。")]
    public required string Name { get; set; }
    [Range(1, 150, ErrorMessage = "年龄必须在1到150岁之间。")]
    public int Age { get; set; }
}

[ApiController]
[Route("talearc/api/[controller]")]
public class TestController(ILogger<TestController> logger) : ControllerBase
{
    private readonly ILogger<TestController> _logger = logger;
    [HttpPost("greet")] 
    public IActionResult Greet([FromBody] GreetingForm request)
    {
        _logger.LogInformation("收到问候请求: 姓名={Name}, 年龄={Age}", request.Name, request.Age);

        string responseMessage = $"你好，{request.Name}！你今年 {request.Age} 岁了。你的请求已成功处理。";
 
        var response = ApiResponse.Success(responseMessage);
        _logger.LogInformation("问候请求处理成功");
        
        return Ok(response); 
    }
    // 你也可以创建一个明确失败的例子
    [HttpGet("fail-test")]
    public IActionResult FailTest()
    {
        _logger.LogWarning("模拟失败测试被调用");
  
        var errorResponse = ApiResponse.Fail(500, "这是一个模拟的服务器内部错误。");

        return StatusCode(500, errorResponse);
    }
}
