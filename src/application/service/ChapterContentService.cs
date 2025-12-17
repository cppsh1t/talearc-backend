namespace talearc_backend.src.application.service;

/// <summary>
/// 章节内容文件服务
/// </summary>
public class ChapterContentService
{
    private readonly string _contentBasePath;
    private readonly ILogger<ChapterContentService> _logger;

    public ChapterContentService(IConfiguration configuration, ILogger<ChapterContentService> logger)
    {
        _contentBasePath = configuration["ContentStorage:BasePath"] ?? "content";
        _logger = logger;
        
        // 确保基础目录存在
        if (!Directory.Exists(_contentBasePath))
        {
            Directory.CreateDirectory(_contentBasePath);
        }
    }

    /// <summary>
    /// 获取章节内容文件路径
    /// </summary>
    private string GetChapterFilePath(int userId, int worldViewId, int novelId, string chapterUuid)
    {
        return Path.Combine(_contentBasePath, userId.ToString(), worldViewId.ToString(), 
            novelId.ToString(), $"{chapterUuid}.txt");
    }

    /// <summary>
    /// 读取章节内容
    /// </summary>
    public async Task<string> ReadChapterContentAsync(int userId, int worldViewId, int novelId, string chapterUuid)
    {
        var filePath = GetChapterFilePath(userId, worldViewId, novelId, chapterUuid);
        
        if (!File.Exists(filePath))
        {
            _logger.LogWarning("章节文件不存在: {FilePath}", filePath);
            return string.Empty;
        }

        try
        {
            return await File.ReadAllTextAsync(filePath);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "读取章节内容失败: {FilePath}", filePath);
            throw;
        }
    }

    /// <summary>
    /// 写入章节内容
    /// </summary>
    public async Task WriteChapterContentAsync(int userId, int worldViewId, int novelId, string chapterUuid, string content)
    {
        var filePath = GetChapterFilePath(userId, worldViewId, novelId, chapterUuid);
        var directory = Path.GetDirectoryName(filePath);

        if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }

        try
        {
            await File.WriteAllTextAsync(filePath, content);
            _logger.LogInformation("章节内容已保存: {FilePath}", filePath);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "写入章节内容失败: {FilePath}", filePath);
            throw;
        }
    }

    /// <summary>
    /// 删除章节内容
    /// </summary>
    public void DeleteChapterContent(int userId, int worldViewId, int novelId, string chapterUuid)
    {
        var filePath = GetChapterFilePath(userId, worldViewId, novelId, chapterUuid);

        if (File.Exists(filePath))
        {
            try
            {
                File.Delete(filePath);
                _logger.LogInformation("章节内容已删除: {FilePath}", filePath);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "删除章节内容失败: {FilePath}", filePath);
                throw;
            }
        }
    }
}

