using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using talearc_backend.src.application.controllers.common;
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
public class MiscController : AuthenticatedControllerBase
{
    public MiscController(AppDbContext context, ILogger<MiscController> logger)
        : base(context, logger)
    {
    }

    /// <summary>
    /// 获取用户的杂项（分页）
    /// </summary>
    /// <param name="request">分页请求（包含 page, size, worldViewId）</param>
    /// <returns>分页的杂项列表</returns>
    /// <response code="200">返回杂项列表</response>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<MiscResponse>>), 200)]
    public async Task<IActionResult> GetMiscs([FromQuery] MiscPagedRequest request)
    {
        var query = Context.Miscs.Where(m => m.UserId == CurrentUserId);
        
        if (request.WorldViewId.HasValue)
        {
            query = query.Where(m => m.WorldViewId == request.WorldViewId.Value);
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
    /// <param name="id">杂项ID</param>
    /// <returns>杂项详情</returns>
    /// <response code="200">返回杂项详情</response>
    /// <response code="404">杂项不存在</response>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ApiResponse<MiscResponse>), 200)]
    [ProducesResponseType(typeof(ApiResponse<object>), 404)]
    public async Task<IActionResult> GetMisc(int id)
    {
        var misc = await Context.Miscs
            .Where(m => m.Id == id && m.UserId == CurrentUserId)
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
    /// <param name="request">创建杂项请求</param>
    /// <returns>创建的杂项信息</returns>
    /// <response code="201">创建成功</response>
    /// <response code="400">世界观不存在或无权访问</response>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<MiscResponse>), 201)]
    [ProducesResponseType(typeof(ApiResponse<object>), 400)]
    public async Task<IActionResult> CreateMisc([FromBody] CreateMiscRequest request)
    {
        // 验证世界观是否属于当前用户
        var worldViewExists = await Context.WorldViews
            .AnyAsync(w => w.Id == request.WorldViewId && w.UserId == CurrentUserId);

        if (!worldViewExists)
        {
            return BadRequest(ApiResponse.Fail(400, "世界观不存在或无权访问"));
        }

        var misc = new Misc
        {
            UserId = CurrentUserId,
            WorldViewId = request.WorldViewId,
            Name = request.Name,
            Description = request.Description,
            Type = request.Type,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        Context.Miscs.Add(misc);
        await Context.SaveChangesAsync();

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
    /// <param name="id">杂项ID</param>
    /// <param name="request">更新杂项请求</param>
    /// <returns>更新后的杂项信息</returns>
    /// <response code="200">更新成功</response>
    /// <response code="404">杂项不存在</response>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(ApiResponse<MiscResponse>), 200)]
    [ProducesResponseType(typeof(ApiResponse<object>), 404)]
    public async Task<IActionResult> UpdateMisc(int id, [FromBody] UpdateMiscRequest request)
    {
        var misc = await Context.Miscs
            .FirstOrDefaultAsync(m => m.Id == id && m.UserId == CurrentUserId);

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

        await Context.SaveChangesAsync();

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
    /// <param name="id">杂项ID</param>
    /// <returns>删除结果</returns>
    /// <response code="200">删除成功</response>
    /// <response code="404">杂项不存在</response>
    [HttpDelete("{id}")]
    [ProducesResponseType(typeof(ApiResponse<object>), 200)]
    [ProducesResponseType(typeof(ApiResponse<object>), 404)]
    public async Task<IActionResult> DeleteMisc(int id)
    {
        var misc = await Context.Miscs
            .FirstOrDefaultAsync(m => m.Id == id && m.UserId == CurrentUserId);

        if (misc == null)
        {
            return NotFound(ApiResponse.Fail(404, "杂项不存在"));
        }

        Context.Miscs.Remove(misc);
        await Context.SaveChangesAsync();

        return Ok(ApiResponse.Success<object?>(null, "删除成功"));
    }
}
