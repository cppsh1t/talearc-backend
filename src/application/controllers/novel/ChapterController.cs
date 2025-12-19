using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using talearc_backend.src.application.controllers.common;
using talearc_backend.src.application.service;
using talearc_backend.src.data;
using talearc_backend.src.data.dto;
using talearc_backend.src.data.entities;
using talearc_backend.src.structure;

namespace talearc_backend.src.application.controllers.novel;

/// <summary>
/// 章节控制器
/// </summary>
[ApiController]
[Route("talearc/api/novels/{novelId}/[controller]")]
[Authorize]
public class ChapterController(AppDbContext context, ILogger<ChapterController> logger, ChapterContentService contentService) : AuthenticatedControllerBase(context, logger)
{
    private readonly ChapterContentService _contentService = contentService;

    /// <summary>
    /// 获取章节列表
    /// </summary>
    /// <param name="novelId">小说ID</param>
    /// <returns>章节列表</returns>
    /// <response code="200">返回章节列表</response>
    /// <response code="404">小说不存在</response>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<List<ChapterResponse>>), 200)]
    [ProducesResponseType(typeof(ApiResponse<object>), 404)]
    public async Task<IActionResult> GetChapters(int novelId)
    {
        var novel = await Context.Novels.FirstOrDefaultAsync(n => n.Id == novelId && n.UserId == CurrentUserId);

        if (novel == null)
        {
            return NotFound(ApiResponse.Fail(404, "小说不存在"));
        }

        var chapters = await Context.Chapters
            .Where(c => c.NovelId == novelId)
            .OrderBy(c => c.Order)
            .Select(c => new ChapterResponse
            {
                Id = c.Id,
                Uuid = c.Uuid,
                NovelId = c.NovelId,
                Title = c.Title,
                Summary = c.Summary,
                Order = c.Order,
                ReferencedSnapshotIds = c.ReferencedSnapshotIds,
                ReferencedEventIds = c.ReferencedEventIds,
                ReferencedMiscIds = c.ReferencedMiscIds,
                CreatedAt = c.CreatedAt,
                UpdatedAt = c.UpdatedAt
            })
            .ToListAsync();

        return Ok(ApiResponse.Success(chapters));
    }

    /// <summary>
    /// 获取章节详情（含内容）
    /// </summary>
    /// <param name="novelId">小说ID</param>
    /// <param name="id">章节ID</param>
    /// <returns>章节详情及内容</returns>
    /// <response code="200">返回章节详情</response>
    /// <response code="404">小说或章节不存在</response>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ApiResponse<ChapterResponse>), 200)]
    [ProducesResponseType(typeof(ApiResponse<object>), 404)]
    public async Task<IActionResult> GetChapter(int novelId, int id)
    {
        var novel = await Context.Novels.FirstOrDefaultAsync(n => n.Id == novelId && n.UserId == CurrentUserId);

        if (novel == null)
        {
            return NotFound(ApiResponse.Fail(404, "小说不存在"));
        }

        var chapter = await Context.Chapters.FirstOrDefaultAsync(c => c.Id == id && c.NovelId == novelId);

        if (chapter == null)
        {
            return NotFound(ApiResponse.Fail(404, "章节不存在"));
        }

        var content = await _contentService.ReadChapterContentAsync(CurrentUserId, novel.WorldViewId, novelId, chapter.Uuid);

        var response = new ChapterResponse
        {
            Id = chapter.Id,
            Uuid = chapter.Uuid,
            NovelId = chapter.NovelId,
            Title = chapter.Title,
            Summary = chapter.Summary,
            Order = chapter.Order,
            ReferencedSnapshotIds = chapter.ReferencedSnapshotIds,
            ReferencedEventIds = chapter.ReferencedEventIds,
            ReferencedMiscIds = chapter.ReferencedMiscIds,
            CreatedAt = chapter.CreatedAt,
            UpdatedAt = chapter.UpdatedAt,
            Content = content
        };

        return Ok(ApiResponse.Success(response));
    }

    /// <summary>
    /// 创建章节
    /// </summary>
    /// <param name="novelId">小说ID</param>
    /// <param name="request">章节信息</param>
    /// <returns>创建的章节</returns>
    /// <response code="200">创建成功</response>
    /// <response code="404">小说不存在</response>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<Chapter>), 200)]
    [ProducesResponseType(typeof(ApiResponse<object>), 404)]
    public async Task<IActionResult> CreateChapter(int novelId, [FromBody] ChapterRequest request)
    {
        var novel = await Context.Novels.FirstOrDefaultAsync(n => n.Id == novelId && n.UserId == CurrentUserId);

        if (novel == null)
        {
            return NotFound(ApiResponse.Fail(404, "小说不存在"));
        }

        var chapter = new Chapter
        {
            Uuid = Guid.NewGuid().ToString(),
            NovelId = novelId,
            Title = request.Title,
            Summary = request.Summary,
            Order = request.Order,
            ReferencedSnapshotIds = request.ReferencedSnapshotIds,
            ReferencedEventIds = request.ReferencedEventIds,
            ReferencedMiscIds = request.ReferencedMiscIds,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        Context.Chapters.Add(chapter);
        await Context.SaveChangesAsync();

        // 保存章节内容到文件
        await _contentService.WriteChapterContentAsync(CurrentUserId, novel.WorldViewId, novelId, chapter.Uuid, request.Content);

        Logger.LogInformation("章节已创建: {ChapterId}", chapter.Id);
        return Ok(ApiResponse.Success(chapter, "创建成功"));
    }

    /// <summary>
    /// 更新章节
    /// </summary>
    /// <param name="novelId">小说ID</param>
    /// <param name="id">章节ID</param>
    /// <param name="request">更新的章节信息</param>
    /// <returns>更新后的章节</returns>
    /// <response code="200">更新成功</response>
    /// <response code="404">小说或章节不存在</response>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(ApiResponse<Chapter>), 200)]
    [ProducesResponseType(typeof(ApiResponse<object>), 404)]
    public async Task<IActionResult> UpdateChapter(int novelId, int id, [FromBody] ChapterRequest request)
    {
        var novel = await Context.Novels.FirstOrDefaultAsync(n => n.Id == novelId && n.UserId == CurrentUserId);

        if (novel == null)
        {
            return NotFound(ApiResponse.Fail(404, "小说不存在"));
        }

        var chapter = await Context.Chapters.FirstOrDefaultAsync(c => c.Id == id && c.NovelId == novelId);

        if (chapter == null)
        {
            return NotFound(ApiResponse.Fail(404, "章节不存在"));
        }

        chapter.Title = request.Title;
        chapter.Summary = request.Summary;
        chapter.Order = request.Order;
        chapter.ReferencedSnapshotIds = request.ReferencedSnapshotIds;
        chapter.ReferencedEventIds = request.ReferencedEventIds;
        chapter.ReferencedMiscIds = request.ReferencedMiscIds;
        chapter.UpdatedAt = DateTime.UtcNow;

        await Context.SaveChangesAsync();

        // 更新章节内容文件
        await _contentService.WriteChapterContentAsync(CurrentUserId, novel.WorldViewId, novelId, chapter.Uuid, request.Content);

        Logger.LogInformation("章节已更新: {ChapterId}", id);
        return Ok(ApiResponse.Success(chapter, "更新成功"));
    }

    /// <summary>
    /// 删除章节
    /// </summary>
    /// <param name="novelId">小说ID</param>
    /// <param name="id">章节ID</param>
    /// <returns>删除结果</returns>
    /// <response code="200">删除成功</response>
    /// <response code="404">小说或章节不存在</response>
    [HttpDelete("{id}")]
    [ProducesResponseType(typeof(ApiResponse<object>), 200)]
    [ProducesResponseType(typeof(ApiResponse<object>), 404)]
    public async Task<IActionResult> DeleteChapter(int novelId, int id)
    {
        var novel = await Context.Novels.FirstOrDefaultAsync(n => n.Id == novelId && n.UserId == CurrentUserId);

        if (novel == null)
        {
            return NotFound(ApiResponse.Fail(404, "小说不存在"));
        }

        var chapter = await Context.Chapters.FirstOrDefaultAsync(c => c.Id == id && c.NovelId == novelId);

        if (chapter == null)
        {
            return NotFound(ApiResponse.Fail(404, "章节不存在"));
        }

        // 删除章节内容文件
        _contentService.DeleteChapterContent(CurrentUserId, novel.WorldViewId, novelId, chapter.Uuid);

        Context.Chapters.Remove(chapter);
        await Context.SaveChangesAsync();

        Logger.LogInformation("章节已删除: {ChapterId}", id);
        return Ok(ApiResponse.Success<object>(null!, "删除成功"));
    }
}

