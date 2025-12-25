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
/// 角色快照管理控制器
/// </summary>
[Authorize]
[ApiController]
[Route("talearc/api/[controller]")]
public class CharacterSnapshotController(AppDbContext context, ILogger<CharacterSnapshotController> logger) : AuthenticatedControllerBase(context, logger)
{
    /// <summary>
    /// 获取用户的角色快照（分页）
    /// </summary>
    /// <param name="request">分页请求（包含 page, size, worldViewId, characterId）</param>
    /// <returns>分页的角色快照列表</returns>
    /// <response code="200">返回角色快照列表</response>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<CharacterSnapshotResponse>>), 200)]
    public async Task<IActionResult> GetCharacterSnapshots([FromQuery] CharacterSnapshotPagedRequest request)
    {
        var query = Context.CharacterSnapshots.Where(s => s.UserId == CurrentUserId);
        
        if (request.WorldViewId.HasValue)
        {
            query = query.Where(s => s.WorldViewId == request.WorldViewId.Value);
        }

        if (request.CharacterId.HasValue)
        {
            query = query.Where(s => s.CharacterId == request.CharacterId.Value);
        }

        var total = await query.CountAsync();

        var list = await query
            .OrderByDescending(s => s.UpdatedAt)
            .Page(request)
            .Select(s => new CharacterSnapshotResponse
            {
                Id = s.Id,
                UserId = s.UserId,
                WorldViewId = s.WorldViewId,
                CharacterId = s.CharacterId,
                Name = s.Name,
                Description = s.Description,
                Note = s.Note,
                CreatedAt = s.CreatedAt,
                UpdatedAt = s.UpdatedAt
            })
            .ToListAsync();

        var result = new PagedResult<CharacterSnapshotResponse>
        {
            List = list,
            Total = total
        };

        return Ok(ApiResponse.Success(result));
    }

    /// <summary>
    /// 根据ID获取角色快照
    /// </summary>
    /// <param name="id">角色快照ID</param>
    /// <returns>角色快照详情</returns>
    /// <response code="200">返回角色快照详情</response>
    /// <response code="404">角色快照不存在</response>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ApiResponse<CharacterSnapshotResponse>), 200)]
    [ProducesResponseType(typeof(ApiResponse<object>), 404)]
    public async Task<IActionResult> GetCharacterSnapshot(int id)
    {
        var snapshot = await Context.CharacterSnapshots
            .Where(s => s.Id == id && s.UserId == CurrentUserId)
            .Select(s => new CharacterSnapshotResponse
            {
                Id = s.Id,
                UserId = s.UserId,
                WorldViewId = s.WorldViewId,
                CharacterId = s.CharacterId,
                Name = s.Name,
                Description = s.Description,
                Note = s.Note,
                CreatedAt = s.CreatedAt,
                UpdatedAt = s.UpdatedAt
            })
            .FirstOrDefaultAsync();

        if (snapshot == null)
        {
            return NotFound(ApiResponse.Fail(404, "角色快照不存在"));
        }

        return Ok(ApiResponse.Success(snapshot));
    }

    /// <summary>
    /// 创建角色快照
    /// </summary>
    /// <param name="request">创建角色快照请求</param>
    /// <returns>创建的角色快照信息</returns>
    /// <response code="201">创建成功</response>
    /// <response code="400">世界观或角色不存在或无权访问</response>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<CharacterSnapshotResponse>), 201)]
    [ProducesResponseType(typeof(ApiResponse<object>), 400)]
    public async Task<IActionResult> CreateCharacterSnapshot([FromBody] CreateCharacterSnapshotRequest request)
    {
        // 验证世界观是否属于当前用户
        var worldViewExists = await Context.WorldViews
            .AnyAsync(w => w.Id == request.WorldViewId && w.UserId == CurrentUserId);

        if (!worldViewExists)
        {
            return BadRequest(ApiResponse.Fail(400, "世界观不存在或无权访问"));
        }

        // 验证角色是否属于当前用户和指定的世界观
        var characterExists = await Context.Characters
            .AnyAsync(c => c.Id == request.CharacterId && c.UserId == CurrentUserId && c.WorldViewId == request.WorldViewId);

        if (!characterExists)
        {
            return BadRequest(ApiResponse.Fail(400, "角色不存在或无权访问"));
        }

        var snapshot = new CharacterSnapshot
        {
            UserId = CurrentUserId,
            WorldViewId = request.WorldViewId,
            CharacterId = request.CharacterId,
            Name = request.Name,
            Description = request.Description,
            Note = request.Note,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        Context.CharacterSnapshots.Add(snapshot);
        await Context.SaveChangesAsync();

        // 更新角色的快照ID数组
        var character = await Context.Characters.FindAsync(request.CharacterId);
        if (character != null)
        {
            var snapshotIdsList = character.SnapshotIds.ToList();
            snapshotIdsList.Add(snapshot.Id);
            character.SnapshotIds = snapshotIdsList.ToArray();
            character.UpdatedAt = DateTime.UtcNow;
            await Context.SaveChangesAsync();
        }

        var response = new CharacterSnapshotResponse
        {
            Id = snapshot.Id,
            UserId = snapshot.UserId,
            WorldViewId = snapshot.WorldViewId,
            CharacterId = snapshot.CharacterId,
            Name = snapshot.Name,
            Description = snapshot.Description,
            Note = snapshot.Note,
            CreatedAt = snapshot.CreatedAt,
            UpdatedAt = snapshot.UpdatedAt
        };

        Logger.LogInformation("角色快照已创建: {SnapshotId}", snapshot.Id);
        return CreatedAtAction(nameof(GetCharacterSnapshot), new { id = snapshot.Id }, ApiResponse.Success(response));
    }

    /// <summary>
    /// 更新角色快照
    /// </summary>
    /// <param name="id">角色快照ID</param>
    /// <param name="request">更新角色快照请求</param>
    /// <returns>更新后的角色快照信息</returns>
    /// <response code="200">更新成功</response>
    /// <response code="404">角色快照不存在</response>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(ApiResponse<CharacterSnapshotResponse>), 200)]
    [ProducesResponseType(typeof(ApiResponse<object>), 404)]
    public async Task<IActionResult> UpdateCharacterSnapshot(int id, [FromBody] UpdateCharacterSnapshotRequest request)
    {
        var snapshot = await Context.CharacterSnapshots
            .FirstOrDefaultAsync(s => s.Id == id && s.UserId == CurrentUserId);

        if (snapshot == null)
        {
            return NotFound(ApiResponse.Fail(404, "角色快照不存在"));
        }

        if (!string.IsNullOrEmpty(request.Name))
        {
            snapshot.Name = request.Name;
        }

        if (request.Description != null)
        {
            snapshot.Description = request.Description;
        }

        if (request.Note != null)
        {
            snapshot.Note = request.Note;
        }

        snapshot.UpdatedAt = DateTime.UtcNow;

        await Context.SaveChangesAsync();

        var response = new CharacterSnapshotResponse
        {
            Id = snapshot.Id,
            UserId = snapshot.UserId,
            WorldViewId = snapshot.WorldViewId,
            CharacterId = snapshot.CharacterId,
            Name = snapshot.Name,
            Description = snapshot.Description,
            Note = snapshot.Note,
            CreatedAt = snapshot.CreatedAt,
            UpdatedAt = snapshot.UpdatedAt
        };

        Logger.LogInformation("角色快照已更新: {SnapshotId}", id);
        return Ok(ApiResponse.Success(response));
    }

    /// <summary>
    /// 删除角色快照
    /// </summary>
    /// <param name="id">角色快照ID</param>
    /// <returns>删除结果</returns>
    /// <response code="200">删除成功</response>
    /// <response code="404">角色快照不存在</response>
    [HttpDelete("{id}")]
    [ProducesResponseType(typeof(ApiResponse<object>), 200)]
    [ProducesResponseType(typeof(ApiResponse<object>), 404)]
    public async Task<IActionResult> DeleteCharacterSnapshot(int id)
    {
        var snapshot = await Context.CharacterSnapshots
            .FirstOrDefaultAsync(s => s.Id == id && s.UserId == CurrentUserId);

        if (snapshot == null)
        {
            return NotFound(ApiResponse.Fail(404, "角色快照不存在"));
        }

        // 从角色的快照ID数组中移除
        var character = await Context.Characters.FindAsync(snapshot.CharacterId);
        if (character != null)
        {
            character.SnapshotIds = character.SnapshotIds.Where(sid => sid != id).ToArray();
            character.UpdatedAt = DateTime.UtcNow;
        }

        Context.CharacterSnapshots.Remove(snapshot);
        await Context.SaveChangesAsync();

        Logger.LogInformation("角色快照已删除: {SnapshotId}", id);
        return Ok(ApiResponse.Success<object?>(null, "删除成功"));
    }
}

