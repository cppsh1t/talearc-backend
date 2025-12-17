using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace talearc_backend.src.application.service;

/// <summary>
/// JWT Token 生成器，使用 Microsoft.IdentityModel.Tokens 标准库实现
/// </summary>
public class JwtTokenGenerator
{
    private readonly string _secretKey;
    private readonly int _expirationMinutes;

    public JwtTokenGenerator(IConfiguration configuration)
    {
        _secretKey = configuration["Jwt:SecretKey"] ?? throw new InvalidOperationException("Jwt:SecretKey not configured");
        _expirationMinutes = int.Parse(configuration["Jwt:ExpirationMinutes"] ?? "60");
    }

    /// <summary>
    /// 生成 JWT Token
    /// </summary>
    /// <param name="userId">用户 ID</param>
    /// <param name="userName">用户名</param>
    /// <returns>JWT Token 字符串</returns>
    public string GenerateToken(int userId, string userName)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secretKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
            new Claim(ClaimTypes.Name, userName)
        };

        var token = new JwtSecurityToken(
            issuer: "talearc",
            audience: "talearc-api",
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(_expirationMinutes),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}