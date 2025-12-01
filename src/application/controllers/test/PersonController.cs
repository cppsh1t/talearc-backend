using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using talearc_backend.src.data;
using talearc_backend.src.data.entities;
using talearc_backend.src.structure;

namespace talearc_backend.src.application.controllers.test;

[ApiController]
[Route("talearc/api/[controller]")]
public class PersonController(AppDbContext context, ILogger<PersonController> logger) : ControllerBase
{
    private readonly AppDbContext _context = context;
    private readonly ILogger<PersonController> _logger = logger;

    [HttpGet("list")]
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