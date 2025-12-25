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
/// 角色管理控制器
/// </summary>
[Authorize]
[ApiController]
[Route("talearc/api/[controller]")]
public class CharacterController(AppDbContext context, ILogger<CharacterController> logger) : AuthenticatedControllerBase(context, logger)
{
    /// <summary>
    /// 获取用户的角色（分页）
    /// </summary>
    /// <param name="request">分页请求（包含 page, size, worldViewId）</param>
    /// <returns>分页的角色列表</returns>
    /// <response code="200">返回角色列表</response>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<CharacterResponse>>), 200)]
    public async Task<IActionResult> GetCharacters([FromQuery] CharacterPagedRequest request)
    {
        var query = Context.Characters.Where(c => c.UserId == CurrentUserId);
        
        if (request.WorldViewId.HasValue)
        {
            query = query.Where(c => c.WorldViewId == request.WorldViewId.Value);
        }

        var total = await query.CountAsync();

        var list = await query
            .OrderByDescending(c => c.UpdatedAt)
            .Page(request)
            .Select(c => new CharacterResponse
            {
                Id = c.Id,
                UserId = c.UserId,
                WorldViewId = c.WorldViewId,
                SnapshotIds = c.SnapshotIds,
                Name = c.Name,
                Description = c.Description,
                Note = c.Note,
                CreatedAt = c.CreatedAt,
                UpdatedAt = c.UpdatedAt
            })
            .ToListAsync();

        var result = new PagedResult<CharacterResponse>
        {
            List = list,
            Total = total
        };

        return Ok(ApiResponse.Success(result));
    }

    /// <summary>
    /// 根据ID获取角色
    /// </summary>
    /// <param name="id">角色ID</param>
    /// <returns>角色详情</returns>
    /// <response code="200">返回角色详情</response>
    /// <response code="404">角色不存在</response>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ApiResponse<CharacterResponse>), 200)]
    [ProducesResponseType(typeof(ApiResponse<object>), 404)]
    public async Task<IActionResult> GetCharacter(int id)
    {
        var character = await Context.Characters
            .Where(c => c.Id == id && c.UserId == CurrentUserId)
            .Select(c => new CharacterResponse
            {
                Id = c.Id,
                UserId = c.UserId,
                WorldViewId = c.WorldViewId,
                SnapshotIds = c.SnapshotIds,
                Name = c.Name,
                Description = c.Description,
                Note = c.Note,
                CreatedAt = c.CreatedAt,
                UpdatedAt = c.UpdatedAt
            })
            .FirstOrDefaultAsync();

        if (character == null)
        {
            return NotFound(ApiResponse.Fail(404, "角色不存在"));
        }

        return Ok(ApiResponse.Success(character));
    }

    /// <summary>
    /// 创建角色
    /// </summary>
    /// <param name="request">创建角色请求</param>
    /// <returns>创建的角色信息</returns>
    /// <response code="201">创建成功</response>
    /// <response code="400">世界观不存在或无权访问</response>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<CharacterResponse>), 201)]
    [ProducesResponseType(typeof(ApiResponse<object>), 400)]
    public async Task<IActionResult> CreateCharacter([FromBody] CreateCharacterRequest request)
    {
        // 验证世界观是否属于当前用户
        var worldViewExists = await Context.WorldViews
            .AnyAsync(w => w.Id == request.WorldViewId && w.UserId == CurrentUserId);

        if (!worldViewExists)
        {
            return BadRequest(ApiResponse.Fail(400, "世界观不存在或无权访问"));
        }

        var character = new Character
        {
            UserId = CurrentUserId,
            WorldViewId = request.WorldViewId,
            Name = request.Name,
            Description = request.Description,
            Note = request.Note,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        Context.Characters.Add(character);
        await Context.SaveChangesAsync();

        var response = new CharacterResponse
        {
            Id = character.Id,
            UserId = character.UserId,
            WorldViewId = character.WorldViewId,
            SnapshotIds = character.SnapshotIds,
            Name = character.Name,
            Description = character.Description,
            Note = character.Note,
            CreatedAt = character.CreatedAt,
            UpdatedAt = character.UpdatedAt
        };

        Logger.LogInformation("角色已创建: {CharacterId}", character.Id);
        return CreatedAtAction(nameof(GetCharacter), new { id = character.Id }, ApiResponse.Success(response));
    }

    /// <summary>
    /// 更新角色
    /// </summary>
    /// <param name="id">角色ID</param>
    /// <param name="request">更新角色请求</param>
    /// <returns>更新后的角色信息</returns>
    /// <response code="200">更新成功</response>
    /// <response code="404">角色不存在</response>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(ApiResponse<CharacterResponse>), 200)]
    [ProducesResponseType(typeof(ApiResponse<object>), 404)]
    public async Task<IActionResult> UpdateCharacter(int id, [FromBody] UpdateCharacterRequest request)
    {
        var character = await Context.Characters
            .FirstOrDefaultAsync(c => c.Id == id && c.UserId == CurrentUserId);

        if (character == null)
        {
            return NotFound(ApiResponse.Fail(404, "角色不存在"));
        }

        if (!string.IsNullOrEmpty(request.Name))
        {
            character.Name = request.Name;
        }

        if (request.Description != null)
        {
            character.Description = request.Description;
        }

        if (request.Note != null)
        {
            character.Note = request.Note;
        }

        character.UpdatedAt = DateTime.UtcNow;

        await Context.SaveChangesAsync();

        var response = new CharacterResponse
        {
            Id = character.Id,
            UserId = character.UserId,
            WorldViewId = character.WorldViewId,
            SnapshotIds = character.SnapshotIds,
            Name = character.Name,
            Description = character.Description,
            Note = character.Note,
            CreatedAt = character.CreatedAt,
            UpdatedAt = character.UpdatedAt
        };

        Logger.LogInformation("角色已更新: {CharacterId}", id);
        return Ok(ApiResponse.Success(response));
    }

    /// <summary>
    /// 删除角色
    /// </summary>
    /// <param name="id">角色ID</param>
    /// <returns>删除结果</returns>
    /// <response code="200">删除成功</response>
    /// <response code="404">角色不存在</response>
    [HttpDelete("{id}")]
    [ProducesResponseType(typeof(ApiResponse<object>), 200)]
    [ProducesResponseType(typeof(ApiResponse<object>), 404)]
    public async Task<IActionResult> DeleteCharacter(int id)
    {
        var character = await Context.Characters
            .FirstOrDefaultAsync(c => c.Id == id && c.UserId == CurrentUserId);

        if (character == null)
        {
            return NotFound(ApiResponse.Fail(404, "角色不存在"));
        }

        // 删除关联的所有快照
        var snapshots = await Context.CharacterSnapshots
            .Where(s => s.CharacterId == id)
            .ToListAsync();
        Context.CharacterSnapshots.RemoveRange(snapshots);

        Context.Characters.Remove(character);
        await Context.SaveChangesAsync();

        Logger.LogInformation("角色已删除: {CharacterId}", id);
        return Ok(ApiResponse.Success<object?>(null, "删除成功"));
    }
}

