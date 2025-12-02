using Microsoft.EntityFrameworkCore;
using talearc_backend.src.data;
using talearc_backend.src.data.entities;

namespace talearc_backend.src.application.service;

public class TokenBlacklistService
{
    private readonly AppDbContext _context;
    private readonly ILogger<TokenBlacklistService> _logger;

    public TokenBlacklistService(AppDbContext context, ILogger<TokenBlacklistService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task AddToBlacklistAsync(string token, DateTime expiresAt)
    {
        var blacklistEntry = new TokenBlacklist
        {
            Token = token,
            ExpiresAt = expiresAt
        };
        
        _context.TokenBlacklists.Add(blacklistEntry);
        await _context.SaveChangesAsync();
        _logger.LogInformation("Token 已添加到黑名单");
    }

    public async Task<bool> IsTokenBlacklistedAsync(string token)
    {
        return await _context.TokenBlacklists
            .AnyAsync(t => t.Token == token && t.ExpiresAt > DateTime.UtcNow);
    }

    public async Task CleanupExpiredTokensAsync()
    {
        var expiredTokens = _context.TokenBlacklists
            .Where(t => t.ExpiresAt <= DateTime.UtcNow);
        
        _context.TokenBlacklists.RemoveRange(expiredTokens);
        await _context.SaveChangesAsync();
        _logger.LogInformation("已清理过期的黑名单 Token");
    }
}