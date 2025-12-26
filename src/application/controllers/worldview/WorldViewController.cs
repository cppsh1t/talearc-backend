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
/// 世界观管理控制器
/// </summary>
[Authorize]
[ApiController]
[Route("talearc/api/[controller]")]
public class WorldViewController(AppDbContext context, ILogger<WorldViewController> logger) : AuthenticatedControllerBase(context, logger)
{
    /// <summary>
    /// 获取用户的世界观（分页）
    /// </summary>
    /// <param name="request">分页请求（包含 page, size）</param>
    /// <returns>分页的世界观列表</returns>
    /// <response code="200">返回世界观列表</response>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<WorldViewResponse>>), 200)]
    public async Task<IActionResult> GetWorldViews([FromQuery] WorldViewPagedRequest request)
    {
        var query = Context.WorldViews.Where(w => w.UserId == CurrentUserId);

        var total = await query.CountAsync();

        var list = await query
            .OrderByDescending(w => w.UpdatedAt)
            .Page(request)
            .Select(w => new WorldViewResponse
            {
                Id = w.Id,
                UserId = w.UserId,
                CharacterIds = w.CharacterIds,
                MiscIds = w.MiscIds,
                WorldEventIds = w.WorldEventIds,
                Name = w.Name,
                Description = w.Description,
                Notes = w.Notes,
                CreatedAt = w.CreatedAt,
                UpdatedAt = w.UpdatedAt
            })
            .ToListAsync();

        var result = new PagedResult<WorldViewResponse>
        {
            List = list,
            Total = total
        };

        return Ok(ApiResponse.Success(result));
    }

    /// <summary>
    /// 根据ID获取世界观
    /// </summary>
    /// <param name="id">世界观ID</param>
    /// <returns>世界观详情</returns>
    /// <response code="200">返回世界观详情</response>
    /// <response code="404">世界观不存在</response>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ApiResponse<WorldViewResponse>), 200)]
    [ProducesResponseType(typeof(ApiResponse<object>), 404)]
    public async Task<IActionResult> GetWorldView(int id)
    {
        var worldView = await Context.WorldViews
            .Where(w => w.Id == id && w.UserId == CurrentUserId)
            .Select(w => new WorldViewResponse
            {
                Id = w.Id,
                UserId = w.UserId,
                CharacterIds = w.CharacterIds,
                MiscIds = w.MiscIds,
                WorldEventIds = w.WorldEventIds,
                Name = w.Name,
                Description = w.Description,
                Notes = w.Notes,
                CreatedAt = w.CreatedAt,
                UpdatedAt = w.UpdatedAt
            })
            .FirstOrDefaultAsync();

        if (worldView == null)
        {
            return NotFound(ApiResponse.Fail(404, "世界观不存在"));
        }

        return Ok(ApiResponse.Success(worldView));
    }

    /// <summary>
    /// 创建世界观
    /// </summary>
    /// <param name="request">创建世界观请求</param>
    /// <returns>创建的世界观信息</returns>
    /// <response code="201">创建成功</response>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<WorldViewResponse>), 201)]
    public async Task<IActionResult> CreateWorldView([FromBody] CreateWorldViewRequest request)
    {
        var worldView = new WorldView
        {
            UserId = CurrentUserId,
            Name = request.Name,
            Description = request.Description,
            Notes = request.Notes,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        Context.WorldViews.Add(worldView);
        await Context.SaveChangesAsync();

        var response = new WorldViewResponse
        {
            Id = worldView.Id,
            UserId = worldView.UserId,
            CharacterIds = worldView.CharacterIds,
            MiscIds = worldView.MiscIds,
            WorldEventIds = worldView.WorldEventIds,
            Name = worldView.Name,
            Description = worldView.Description,
            Notes = worldView.Notes,
            CreatedAt = worldView.CreatedAt,
            UpdatedAt = worldView.UpdatedAt
        };

        Logger.LogInformation("世界观已创建: {WorldViewId}", worldView.Id);
        return CreatedAtAction(nameof(GetWorldView), new { id = worldView.Id }, ApiResponse.Success(response));
    }

    /// <summary>
    /// 更新世界观
    /// </summary>
    /// <param name="id">世界观ID</param>
    /// <param name="request">更新世界观请求</param>
    /// <returns>更新后的世界观信息</returns>
    /// <response code="200">更新成功</response>
    /// <response code="404">世界观不存在</response>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(ApiResponse<WorldViewResponse>), 200)]
    [ProducesResponseType(typeof(ApiResponse<object>), 404)]
    public async Task<IActionResult> UpdateWorldView(int id, [FromBody] UpdateWorldViewRequest request)
    {
        var worldView = await Context.WorldViews
            .FirstOrDefaultAsync(w => w.Id == id && w.UserId == CurrentUserId);

        if (worldView == null)
        {
            return NotFound(ApiResponse.Fail(404, "世界观不存在"));
        }

        if (!string.IsNullOrEmpty(request.Name))
        {
            worldView.Name = request.Name;
        }

        if (request.Description != null)
        {
            worldView.Description = request.Description;
        }

        if (request.Notes != null)
        {
            worldView.Notes = request.Notes;
        }

        worldView.UpdatedAt = DateTime.UtcNow;

        await Context.SaveChangesAsync();

        var response = new WorldViewResponse
        {
            Id = worldView.Id,
            UserId = worldView.UserId,
            CharacterIds = worldView.CharacterIds,
            MiscIds = worldView.MiscIds,
            WorldEventIds = worldView.WorldEventIds,
            Name = worldView.Name,
            Description = worldView.Description,
            Notes = worldView.Notes,
            CreatedAt = worldView.CreatedAt,
            UpdatedAt = worldView.UpdatedAt
        };

        Logger.LogInformation("世界观已更新: {WorldViewId}", id);
        return Ok(ApiResponse.Success(response));
    }

    /// <summary>
    /// 删除世界观
    /// </summary>
    /// <param name="id">世界观ID</param>
    /// <returns>删除结果</returns>
    /// <response code="200">删除成功</response>
    /// <response code="404">世界观不存在</response>
    [HttpDelete("{id}")]
    [ProducesResponseType(typeof(ApiResponse<object>), 200)]
    [ProducesResponseType(typeof(ApiResponse<object>), 404)]
    public async Task<IActionResult> DeleteWorldView(int id)
    {
        var worldView = await Context.WorldViews
            .FirstOrDefaultAsync(w => w.Id == id && w.UserId == CurrentUserId);

        if (worldView == null)
        {
            return NotFound(ApiResponse.Fail(404, "世界观不存在"));
        }

        // 删除关联的所有角色
        var characters = await Context.Characters
            .Where(c => c.WorldViewId == id)
            .ToListAsync();
        Context.Characters.RemoveRange(characters);

        // 删除关联的所有角色快照
        var snapshots = await Context.CharacterSnapshots
            .Where(s => s.WorldViewId == id)
            .ToListAsync();
        Context.CharacterSnapshots.RemoveRange(snapshots);

        // 删除关联的所有世界事件
        var worldEvents = await Context.WorldEvents
            .Where(e => e.WorldViewId == id)
            .ToListAsync();
        Context.WorldEvents.RemoveRange(worldEvents);

        // 删除关联的所有杂项
        var miscs = await Context.Miscs
            .Where(m => m.WorldViewId == id)
            .ToListAsync();
        Context.Miscs.RemoveRange(miscs);

        // 删除关联的所有小说
        var novels = await Context.Novels
            .Where(n => n.WorldViewId == id)
            .ToListAsync();
        Context.Novels.RemoveRange(novels);

        Context.WorldViews.Remove(worldView);
        await Context.SaveChangesAsync();

        Logger.LogInformation("世界观已删除: {WorldViewId}", id);
        return Ok(ApiResponse.Success<object?>(null, "删除成功"));
    }
}

