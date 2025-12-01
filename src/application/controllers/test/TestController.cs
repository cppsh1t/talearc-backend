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
[Route("api/[controller]")]
public class TestController : ControllerBase
{
   [HttpPost("greet")] 
    public IActionResult Greet([FromBody] GreetingForm request)
    {

        string responseMessage = $"你好，{request.Name}！你今年 {request.Age} 岁了。你的请求已成功处理。";
 
        var response = ApiResponse<string>.Success(responseMessage);
        
        return Ok(response); 
    }
    // 你也可以创建一个明确失败的例子
    [HttpGet("fail-test")]
    public IActionResult FailTest()
    {
  
        var errorResponse = ApiResponse.Fail(500, "这是一个模拟的服务器内部错误。");

        return StatusCode(500, errorResponse);
    }
}
