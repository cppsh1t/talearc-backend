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
/// 世界事件管理控制器
/// </summary>
[Authorize]
[ApiController]
[Route("talearc/api/[controller]")]
public class WorldEventController(AppDbContext context, ILogger<WorldEventController> logger) : AuthenticatedControllerBase(context, logger)
{
    /// <summary>
    /// 获取用户的世界事件（分页）
    /// </summary>
    /// <param name="request">分页请求（包含 page, size, worldViewId）</param>
    /// <returns>分页的世界事件列表</returns>
    /// <response code="200">返回世界事件列表</response>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<WorldEventResponse>>), 200)]
    public async Task<IActionResult> GetWorldEvents([FromQuery] WorldEventPagedRequest request)
    {
        var query = Context.WorldEvents.Where(e => e.UserId == CurrentUserId);
        
        if (request.WorldViewId.HasValue)
        {
            query = query.Where(e => e.WorldViewId == request.WorldViewId.Value);
        }

        var total = await query.CountAsync();

        var list = await query
            .OrderByDescending(e => e.HappenedAt)
            .Page(request)
            .Select(e => new WorldEventResponse
            {
                Id = e.Id,
                UserId = e.UserId,
                WorldViewId = e.WorldViewId,
                Name = e.Name,
                Description = e.Description,
                HappenedAt = e.HappenedAt,
                EndAt = e.EndAt,
                RelatedCharacterSnapshotIds = e.RelatedCharacterSnapshotIds,
                CreatedAt = e.CreatedAt,
                UpdatedAt = e.UpdatedAt
            })
            .ToListAsync();

        var result = new PagedResult<WorldEventResponse>
        {
            List = list,
            Total = total
        };

        return Ok(ApiResponse.Success(result));
    }

    /// <summary>
    /// 获取指定世界观下的所有世界事件
    /// </summary>
    /// <param name="worldViewId">世界观ID</param>
    /// <returns>世界事件列表</returns>
    /// <response code="200">返回世界事件列表</response>
    /// <response code="404">世界观不存在或无权访问</response>
    [HttpGet("list/{worldViewId}")]
    [ProducesResponseType(typeof(ApiResponse<List<WorldEventResponse>>), 200)]
    [ProducesResponseType(typeof(ApiResponse<object>), 404)]
    public async Task<IActionResult> GetWorldEventsByWorldViewId(int worldViewId)
    {
        // 验证世界观是否属于当前用户
        var worldViewExists = await Context.WorldViews
            .AnyAsync(w => w.Id == worldViewId && w.UserId == CurrentUserId);

        if (!worldViewExists)
        {
            return NotFound(ApiResponse.Fail(404, "世界观不存在或无权访问"));
        }

        var list = await Context.WorldEvents
            .Where(e => e.WorldViewId == worldViewId && e.UserId == CurrentUserId)
            .OrderByDescending(e => e.HappenedAt)
            .Select(e => new WorldEventResponse
            {
                Id = e.Id,
                UserId = e.UserId,
                WorldViewId = e.WorldViewId,
                Name = e.Name,
                Description = e.Description,
                HappenedAt = e.HappenedAt,
                EndAt = e.EndAt,
                RelatedCharacterSnapshotIds = e.RelatedCharacterSnapshotIds,
                CreatedAt = e.CreatedAt,
                UpdatedAt = e.UpdatedAt
            })
            .ToListAsync();

        return Ok(ApiResponse.Success(list));
    }

    /// <summary>
    /// 根据ID获取世界事件
    /// </summary>
    /// <param name="id">世界事件ID</param>
    /// <returns>世界事件详情</returns>
    /// <response code="200">返回世界事件详情</response>
    /// <response code="404">世界事件不存在</response>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ApiResponse<WorldEventResponse>), 200)]
    [ProducesResponseType(typeof(ApiResponse<object>), 404)]
    public async Task<IActionResult> GetWorldEvent(int id)
    {
        var worldEvent = await Context.WorldEvents
            .Where(e => e.Id == id && e.UserId == CurrentUserId)
            .Select(e => new WorldEventResponse
            {
                Id = e.Id,
                UserId = e.UserId,
                WorldViewId = e.WorldViewId,
                Name = e.Name,
                Description = e.Description,
                HappenedAt = e.HappenedAt,
                EndAt = e.EndAt,
                RelatedCharacterSnapshotIds = e.RelatedCharacterSnapshotIds,
                CreatedAt = e.CreatedAt,
                UpdatedAt = e.UpdatedAt
            })
            .FirstOrDefaultAsync();

        if (worldEvent == null)
        {
            return NotFound(ApiResponse.Fail(404, "世界事件不存在"));
        }

        return Ok(ApiResponse.Success(worldEvent));
    }

    /// <summary>
    /// 创建世界事件
    /// </summary>
    /// <param name="request">创建世界事件请求</param>
    /// <returns>创建的世界事件信息</returns>
    /// <response code="201">创建成功</response>
    /// <response code="400">世界观不存在或无权访问</response>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<WorldEventResponse>), 201)]
    [ProducesResponseType(typeof(ApiResponse<object>), 400)]
    public async Task<IActionResult> CreateWorldEvent([FromBody] CreateWorldEventRequest request)
    {
        // 验证世界观是否属于当前用户
        var worldViewExists = await Context.WorldViews
            .AnyAsync(w => w.Id == request.WorldViewId && w.UserId == CurrentUserId);

        if (!worldViewExists)
        {
            return BadRequest(ApiResponse.Fail(400, "世界观不存在或无权访问"));
        }

        var worldEvent = new WorldEvent
        {
            UserId = CurrentUserId,
            WorldViewId = request.WorldViewId,
            Name = request.Name,
            Description = request.Description,
            HappenedAt = request.HappenedAt,
            EndAt = request.EndAt,
            RelatedCharacterSnapshotIds = request.RelatedCharacterSnapshotIds,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        Context.WorldEvents.Add(worldEvent);
        await Context.SaveChangesAsync();

        var response = new WorldEventResponse
        {
            Id = worldEvent.Id,
            UserId = worldEvent.UserId,
            WorldViewId = worldEvent.WorldViewId,
            Name = worldEvent.Name,
            Description = worldEvent.Description,
            HappenedAt = worldEvent.HappenedAt,
            EndAt = worldEvent.EndAt,
            RelatedCharacterSnapshotIds = worldEvent.RelatedCharacterSnapshotIds,
            CreatedAt = worldEvent.CreatedAt,
            UpdatedAt = worldEvent.UpdatedAt
        };

        Logger.LogInformation("世界事件已创建: {WorldEventId}", worldEvent.Id);
        return CreatedAtAction(nameof(GetWorldEvent), new { id = worldEvent.Id }, ApiResponse.Success(response));
    }

    /// <summary>
    /// 更新世界事件
    /// </summary>
    /// <param name="id">世界事件ID</param>
    /// <param name="request">更新世界事件请求</param>
    /// <returns>更新后的世界事件信息</returns>
    /// <response code="200">更新成功</response>
    /// <response code="404">世界事件不存在</response>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(ApiResponse<WorldEventResponse>), 200)]
    [ProducesResponseType(typeof(ApiResponse<object>), 404)]
    public async Task<IActionResult> UpdateWorldEvent(int id, [FromBody] UpdateWorldEventRequest request)
    {
        var worldEvent = await Context.WorldEvents
            .FirstOrDefaultAsync(e => e.Id == id && e.UserId == CurrentUserId);

        if (worldEvent == null)
        {
            return NotFound(ApiResponse.Fail(404, "世界事件不存在"));
        }

        if (!string.IsNullOrEmpty(request.Name))
        {
            worldEvent.Name = request.Name;
        }

        if (request.Description != null)
        {
            worldEvent.Description = request.Description;
        }

        if (request.HappenedAt.HasValue)
        {
            worldEvent.HappenedAt = request.HappenedAt.Value;
        }

        if (request.EndAt.HasValue)
        {
            worldEvent.EndAt = request.EndAt.Value;
        }

        if (request.RelatedCharacterSnapshotIds != null)
        {
            worldEvent.RelatedCharacterSnapshotIds = request.RelatedCharacterSnapshotIds;
        }

        worldEvent.UpdatedAt = DateTime.UtcNow;

        await Context.SaveChangesAsync();

        var response = new WorldEventResponse
        {
            Id = worldEvent.Id,
            UserId = worldEvent.UserId,
            WorldViewId = worldEvent.WorldViewId,
            Name = worldEvent.Name,
            Description = worldEvent.Description,
            HappenedAt = worldEvent.HappenedAt,
            EndAt = worldEvent.EndAt,
            RelatedCharacterSnapshotIds = worldEvent.RelatedCharacterSnapshotIds,
            CreatedAt = worldEvent.CreatedAt,
            UpdatedAt = worldEvent.UpdatedAt
        };

        Logger.LogInformation("世界事件已更新: {WorldEventId}", id);
        return Ok(ApiResponse.Success(response));
    }

    /// <summary>
    /// 删除世界事件
    /// </summary>
    /// <param name="id">世界事件ID</param>
    /// <returns>删除结果</returns>
    /// <response code="200">删除成功</response>
    /// <response code="404">世界事件不存在</response>
    [HttpDelete("{id}")]
    [ProducesResponseType(typeof(ApiResponse<object>), 200)]
    [ProducesResponseType(typeof(ApiResponse<object>), 404)]
    public async Task<IActionResult> DeleteWorldEvent(int id)
    {
        var worldEvent = await Context.WorldEvents
            .FirstOrDefaultAsync(e => e.Id == id && e.UserId == CurrentUserId);

        if (worldEvent == null)
        {
            return NotFound(ApiResponse.Fail(404, "世界事件不存在"));
        }

        Context.WorldEvents.Remove(worldEvent);
        await Context.SaveChangesAsync();

        Logger.LogInformation("世界事件已删除: {WorldEventId}", id);
        return Ok(ApiResponse.Success<object?>(null, "删除成功"));
    }
}

