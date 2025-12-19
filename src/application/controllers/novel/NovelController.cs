using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using talearc_backend.src.application.controllers.common;
using talearc_backend.src.application.service;
using talearc_backend.src.data;
using talearc_backend.src.data.entities;
using talearc_backend.src.structure;

namespace talearc_backend.src.application.controllers.novel;

/// <summary>
/// 小说控制器
/// </summary>
[ApiController]
[Route("talearc/api/[controller]")]
[Authorize]
public class NovelController(AppDbContext context, ILogger<NovelController> logger, ChapterContentService contentService) : AuthenticatedControllerBase(context, logger)
{
    private readonly ChapterContentService _contentService = contentService;

    /// <summary>
    /// 获取小说列表
    /// </summary>
    /// <param name="worldViewId">世界观ID（可选）</param>
    /// <returns>小说列表</returns>
    /// <response code="200">返回小说列表</response>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<List<Novel>>), 200)]
    public async Task<IActionResult> GetNovels([FromQuery] int? worldViewId)
    {
        var query = Context.Novels.Where(n => n.UserId == CurrentUserId);

        if (worldViewId.HasValue)
        {
            query = query.Where(n => n.WorldViewId == worldViewId.Value);
        }

        var novels = await query.OrderByDescending(n => n.UpdatedAt).ToListAsync();
        return Ok(ApiResponse.Success(novels));
    }

    /// <summary>
    /// 获取小说详情
    /// </summary>
    /// <param name="id">小说ID</param>
    /// <returns>小说详情</returns>
    /// <response code="200">返回小说详情</response>
    /// <response code="404">小说不存在</response>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ApiResponse<Novel>), 200)]
    [ProducesResponseType(typeof(ApiResponse<object>), 404)]
    public async Task<IActionResult> GetNovel(int id)
    {
        var novel = await Context.Novels.FirstOrDefaultAsync(n => n.Id == id && n.UserId == CurrentUserId);

        if (novel == null)
        {
            return NotFound(ApiResponse.Fail(404, "小说不存在"));
        }

        return Ok(ApiResponse.Success(novel));
    }

    /// <summary>
    /// 创建小说
    /// </summary>
    /// <param name="novel">小说信息</param>
    /// <returns>创建的小说</returns>
    /// <response code="200">创建成功</response>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<Novel>), 200)]
    public async Task<IActionResult> CreateNovel([FromBody] Novel novel)
    {
        novel.UserId = CurrentUserId;
        novel.CreatedAt = DateTime.UtcNow;
        novel.UpdatedAt = DateTime.UtcNow;

        Context.Novels.Add(novel);
        await Context.SaveChangesAsync();

        Logger.LogInformation("小说已创建: {NovelId}", novel.Id);
        return Ok(ApiResponse.Success(novel, "创建成功"));
    }

    /// <summary>
    /// 更新小说
    /// </summary>
    /// <param name="id">小说ID</param>
    /// <param name="updatedNovel">更新的小说信息</param>
    /// <returns>更新后的小说</returns>
    /// <response code="200">更新成功</response>
    /// <response code="404">小说不存在</response>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(ApiResponse<Novel>), 200)]
    [ProducesResponseType(typeof(ApiResponse<object>), 404)]
    public async Task<IActionResult> UpdateNovel(int id, [FromBody] Novel updatedNovel)
    {
        var novel = await Context.Novels.FirstOrDefaultAsync(n => n.Id == id && n.UserId == CurrentUserId);

        if (novel == null)
        {
            return NotFound(ApiResponse.Fail(404, "小说不存在"));
        }

        novel.Title = updatedNovel.Title;
        novel.Description = updatedNovel.Description;
        novel.WorldViewId = updatedNovel.WorldViewId;
        novel.UpdatedAt = DateTime.UtcNow;

        await Context.SaveChangesAsync();
        Logger.LogInformation("小说已更新: {NovelId}", id);
        return Ok(ApiResponse.Success(novel, "更新成功"));
    }

    /// <summary>
    /// 删除小说
    /// </summary>
    /// <param name="id">小说ID</param>
    /// <returns>删除结果</returns>
    /// <response code="200">删除成功</response>
    /// <response code="404">小说不存在</response>
    [HttpDelete("{id}")]
    [ProducesResponseType(typeof(ApiResponse<object>), 200)]
    [ProducesResponseType(typeof(ApiResponse<object>), 404)]
    public async Task<IActionResult> DeleteNovel(int id)
    {
        var novel = await Context.Novels.FirstOrDefaultAsync(n => n.Id == id && n.UserId == CurrentUserId);

        if (novel == null)
        {
            return NotFound(ApiResponse.Fail(404, "小说不存在"));
        }

        // 删除所有章节及其内容文件
        var chapters = await Context.Chapters.Where(c => c.NovelId == id).ToListAsync();
        foreach (var chapter in chapters)
        {
            _contentService.DeleteChapterContent(CurrentUserId, novel.WorldViewId, id, chapter.Uuid);
        }
        Context.Chapters.RemoveRange(chapters);

        Context.Novels.Remove(novel);
        await Context.SaveChangesAsync();

        Logger.LogInformation("小说已删除: {NovelId}", id);
        return Ok(ApiResponse.Success<object>(null!, "删除成功"));
    }
}

