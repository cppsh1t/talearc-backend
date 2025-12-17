using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using talearc_backend.src.data;
using talearc_backend.src.data.entities;
using talearc_backend.src.data.dto;
using talearc_backend.src.structure;
using talearc_backend.src.application.service;

namespace talearc_backend.src.application.controllers.novel;

/// <summary>
/// 章节控制器
/// </summary>
[ApiController]
[Route("talearc/api/novels/{novelId}/[controller]")]
[Authorize]
public class ChapterController(AppDbContext context, ILogger<ChapterController> logger, ChapterContentService contentService) : ControllerBase
{
    private readonly AppDbContext _context = context;
    private readonly ILogger<ChapterController> _logger = logger;
    private readonly ChapterContentService _contentService = contentService;

    private int GetUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return int.TryParse(userIdClaim, out var userId) ? userId : 0;
    }

    /// <summary>
    /// 获取章节列表
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetChapters(int novelId)
    {
        var userId = GetUserId();
        var novel = await _context.Novels.FirstOrDefaultAsync(n => n.Id == novelId && n.UserId == userId);

        if (novel == null)
        {
            return NotFound(ApiResponse.Fail(404, "小说不存在"));
        }

        var chapters = await _context.Chapters
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
    [HttpGet("{id}")]
    public async Task<IActionResult> GetChapter(int novelId, int id)
    {
        var userId = GetUserId();
        var novel = await _context.Novels.FirstOrDefaultAsync(n => n.Id == novelId && n.UserId == userId);

        if (novel == null)
        {
            return NotFound(ApiResponse.Fail(404, "小说不存在"));
        }

        var chapter = await _context.Chapters.FirstOrDefaultAsync(c => c.Id == id && c.NovelId == novelId);

        if (chapter == null)
        {
            return NotFound(ApiResponse.Fail(404, "章节不存在"));
        }

        var content = await _contentService.ReadChapterContentAsync(userId, novel.WorldViewId, novelId, chapter.Uuid);

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
    [HttpPost]
    public async Task<IActionResult> CreateChapter(int novelId, [FromBody] ChapterRequest request)
    {
        var userId = GetUserId();
        var novel = await _context.Novels.FirstOrDefaultAsync(n => n.Id == novelId && n.UserId == userId);

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

        _context.Chapters.Add(chapter);
        await _context.SaveChangesAsync();

        // 保存章节内容到文件
        await _contentService.WriteChapterContentAsync(userId, novel.WorldViewId, novelId, chapter.Uuid, request.Content);

        _logger.LogInformation("章节已创建: {ChapterId}", chapter.Id);
        return Ok(ApiResponse.Success(chapter, "创建成功"));
    }

    /// <summary>
    /// 更新章节
    /// </summary>
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateChapter(int novelId, int id, [FromBody] ChapterRequest request)
    {
        var userId = GetUserId();
        var novel = await _context.Novels.FirstOrDefaultAsync(n => n.Id == novelId && n.UserId == userId);

        if (novel == null)
        {
            return NotFound(ApiResponse.Fail(404, "小说不存在"));
        }

        var chapter = await _context.Chapters.FirstOrDefaultAsync(c => c.Id == id && c.NovelId == novelId);

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

        await _context.SaveChangesAsync();

        // 更新章节内容文件
        await _contentService.WriteChapterContentAsync(userId, novel.WorldViewId, novelId, chapter.Uuid, request.Content);

        _logger.LogInformation("章节已更新: {ChapterId}", id);
        return Ok(ApiResponse.Success(chapter, "更新成功"));
    }

    /// <summary>
    /// 删除章节
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteChapter(int novelId, int id)
    {
        var userId = GetUserId();
        var novel = await _context.Novels.FirstOrDefaultAsync(n => n.Id == novelId && n.UserId == userId);

        if (novel == null)
        {
            return NotFound(ApiResponse.Fail(404, "小说不存在"));
        }

        var chapter = await _context.Chapters.FirstOrDefaultAsync(c => c.Id == id && c.NovelId == novelId);

        if (chapter == null)
        {
            return NotFound(ApiResponse.Fail(404, "章节不存在"));
        }

        // 删除章节内容文件
        _contentService.DeleteChapterContent(userId, novel.WorldViewId, novelId, chapter.Uuid);

        _context.Chapters.Remove(chapter);
        await _context.SaveChangesAsync();

        _logger.LogInformation("章节已删除: {ChapterId}", id);
        return Ok(ApiResponse.Success<object>(null!, "删除成功"));
    }
}

