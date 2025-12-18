using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using talearc_backend.src.data;
using talearc_backend.src.data.dto;
using talearc_backend.src.data.entities;
using talearc_backend.src.structure;

namespace talearc_backend.src.application.controllers.worldview;

/// <summary>
/// 杂项管理控制器
/// </summary>
[Authorize]
[ApiController]
[Route("talearc/api/[controller]")]
public class MiscController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly ILogger<MiscController> _logger;

    public MiscController(AppDbContext context, ILogger<MiscController> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <summary>
    /// 获取用户的杂项（分页）
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetMiscs([FromQuery] PagedRequest<MiscQueryParams> request)
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

        var query = _context.Miscs.Where(m => m.UserId == userId);
        
        if (request.Query?.WorldViewId != null)
        {
            query = query.Where(m => m.WorldViewId == request.Query.WorldViewId.Value);
        }

        var total = await query.CountAsync();

        var list = await query
            .OrderByDescending(m => m.UpdatedAt)
            .Page(request)
            .Select(m => new MiscResponse
            {
                Id = m.Id,
                UserId = m.UserId,
                WorldViewId = m.WorldViewId,
                Name = m.Name,
                Description = m.Description,
                Type = m.Type,
                CreatedAt = m.CreatedAt,
                UpdatedAt = m.UpdatedAt
            })
            .ToListAsync();

        var result = new PagedResult<MiscResponse>
        {
            List = list,
            Total = total
        };

        return Ok(ApiResponse.Success(result));
    }

    /// <summary>
    /// 根据ID获取杂项
    /// </summary>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetMisc(int id)
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

        var misc = await _context.Miscs
            .Where(m => m.Id == id && m.UserId == userId)
            .Select(m => new MiscResponse
            {
                Id = m.Id,
                UserId = m.UserId,
                WorldViewId = m.WorldViewId,
                Name = m.Name,
                Description = m.Description,
                Type = m.Type,
                CreatedAt = m.CreatedAt,
                UpdatedAt = m.UpdatedAt
            })
            .FirstOrDefaultAsync();

        if (misc == null)
        {
            return NotFound(ApiResponse.Fail(404, "杂项不存在"));
        }

        return Ok(ApiResponse.Success(misc));
    }

    /// <summary>
    /// 创建杂项
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> CreateMisc([FromBody] CreateMiscRequest request)
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

        // 验证世界观是否属于当前用户
        var worldViewExists = await _context.WorldViews
            .AnyAsync(w => w.Id == request.WorldViewId && w.UserId == userId);

        if (!worldViewExists)
        {
            return BadRequest(ApiResponse.Fail(400, "世界观不存在或无权访问"));
        }

        var misc = new Misc
        {
            UserId = userId,
            WorldViewId = request.WorldViewId,
            Name = request.Name,
            Description = request.Description,
            Type = request.Type,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.Miscs.Add(misc);
        await _context.SaveChangesAsync();

        var response = new MiscResponse
        {
            Id = misc.Id,
            UserId = misc.UserId,
            WorldViewId = misc.WorldViewId,
            Name = misc.Name,
            Description = misc.Description,
            Type = misc.Type,
            CreatedAt = misc.CreatedAt,
            UpdatedAt = misc.UpdatedAt
        };

        return CreatedAtAction(nameof(GetMisc), new { id = misc.Id }, ApiResponse.Success(response));
    }

    /// <summary>
    /// 更新杂项
    /// </summary>
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateMisc(int id, [FromBody] UpdateMiscRequest request)
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

        var misc = await _context.Miscs
            .FirstOrDefaultAsync(m => m.Id == id && m.UserId == userId);

        if (misc == null)
        {
            return NotFound(ApiResponse.Fail(404, "杂项不存在"));
        }

        if (!string.IsNullOrEmpty(request.Name))
        {
            misc.Name = request.Name;
        }

        if (request.Description != null)
        {
            misc.Description = request.Description;
        }

        if (!string.IsNullOrEmpty(request.Type))
        {
            misc.Type = request.Type;
        }

        misc.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        var response = new MiscResponse
        {
            Id = misc.Id,
            UserId = misc.UserId,
            WorldViewId = misc.WorldViewId,
            Name = misc.Name,
            Description = misc.Description,
            Type = misc.Type,
            CreatedAt = misc.CreatedAt,
            UpdatedAt = misc.UpdatedAt
        };

        return Ok(ApiResponse.Success(response));
    }

    /// <summary>
    /// 删除杂项
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteMisc(int id)
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

        var misc = await _context.Miscs
            .FirstOrDefaultAsync(m => m.Id == id && m.UserId == userId);

        if (misc == null)
        {
            return NotFound(ApiResponse.Fail(404, "杂项不存在"));
        }

        _context.Miscs.Remove(misc);
        await _context.SaveChangesAsync();

        return Ok(ApiResponse.Success<object?>(null, "删除成功"));
    }
}

public class MiscQueryParams
{
    public int? WorldViewId { get; set; }
}
