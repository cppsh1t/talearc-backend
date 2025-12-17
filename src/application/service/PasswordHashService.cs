using System.Security.Cryptography;
using System.Text;

namespace talearc_backend.src.application.service;

/// <summary>
/// 密码哈希服务，使用 PBKDF2 算法提供安全的密码加密
/// </summary>
public class PasswordHashService
{
    private const int SaltSize = 16;
    private const int HashSize = 32;
    private const int Iterations = 100000; // 提高迭代次数以增强安全性（OWASP 推荐 100,000+）

    /// <summary>
    /// 异步哈希密码，使用 PBKDF2 算法
    /// </summary>
    /// <param name="password">明文密码</param>
    /// <returns>Base64 编码的哈希值（包含盐）</returns>
    public async Task<string> HashPasswordAsync(string password)
    {
        // 使用 Task.Run 将 CPU 密集型操作移到线程池线程，避免阻塞主线程
        return await Task.Run(() =>
        {
            using var rng = new Rfc2898DeriveBytes(password, SaltSize, Iterations, HashAlgorithmName.SHA256);
            var salt = rng.Salt;
            var hash = rng.GetBytes(HashSize);
            
            var hashBytes = new byte[SaltSize + HashSize];
            Array.Copy(salt, 0, hashBytes, 0, SaltSize);
            Array.Copy(hash, 0, hashBytes, SaltSize, HashSize);
            
            return Convert.ToBase64String(hashBytes);
        });
    }

    /// <summary>
    /// 异步验证密码
    /// </summary>
    /// <param name="password">待验证的明文密码</param>
    /// <param name="hash">存储的哈希值</param>
    /// <returns>密码是否匹配</returns>
    public async Task<bool> VerifyPasswordAsync(string password, string hash)
    {
        // 使用 Task.Run 将 CPU 密集型操作移到线程池线程，避免阻塞主线程
        return await Task.Run(() =>
        {
            try
            {
                var hashBytes = Convert.FromBase64String(hash);
                var salt = new byte[SaltSize];
                Array.Copy(hashBytes, 0, salt, 0, SaltSize);
                
                using var rng = new Rfc2898DeriveBytes(password, salt, Iterations, HashAlgorithmName.SHA256);
                var computedHash = rng.GetBytes(HashSize);
                
                // 使用固定时间比较，防止时序攻击
                return CryptographicOperations.FixedTimeEquals(
                    hashBytes.AsSpan(SaltSize, HashSize),
                    computedHash
                );
            }
            catch
            {
                return false;
            }
        });
    }
}