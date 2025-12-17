using Microsoft.EntityFrameworkCore;
using talearc_backend.src.data;

namespace talearc_backend.src.application.service;

public class RegistrationKeyService(AppDbContext context, ILogger<RegistrationKeyService> logger)
{
    private readonly AppDbContext _context = context;
    private readonly ILogger<RegistrationKeyService> _logger = logger;


    /// <summary>
    /// 验证注册密钥
    /// </summary>
    public async Task<bool> IsKeyValidAsync(string key)
    {
        return await _context.RegistrationKeys
            .AnyAsync(k => k.Key == key && k.UserId == null);
    }

    /// <summary>
    /// 标记密钥为已使用
    /// </summary>
    public async Task MarkKeyAsUsedAsync(string key, int userId)
    {
        var registrationKey = await _context.RegistrationKeys
            .FirstOrDefaultAsync(k => k.Key == key);
        
        if (registrationKey != null)
        {
            registrationKey.UserId = userId;
            registrationKey.UsedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            _logger.LogInformation("注册密钥已标记为已使用: {Key}, UserId: {UserId}", key, userId);
        }
    }
}