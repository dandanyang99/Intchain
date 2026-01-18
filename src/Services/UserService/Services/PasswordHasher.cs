using Microsoft.AspNetCore.Identity;
using Intchain.UserService.Models;

namespace Intchain.UserService.Services;

/// <summary>
/// 密码哈希服务实现
/// </summary>
public class PasswordHasher : IPasswordHasher
{
    private readonly PasswordHasher<User> _passwordHasher;

    public PasswordHasher()
    {
        _passwordHasher = new PasswordHasher<User>();
    }

    /// <summary>
    /// 哈希密码
    /// </summary>
    public string HashPassword(string password)
    {
        return _passwordHasher.HashPassword(null!, password);
    }

    /// <summary>
    /// 验证密码
    /// </summary>
    public bool VerifyPassword(string hashedPassword, string providedPassword)
    {
        var result = _passwordHasher.VerifyHashedPassword(null!, hashedPassword, providedPassword);
        return result == PasswordVerificationResult.Success;
    }
}
