using Microsoft.EntityFrameworkCore;
using Intchain.UserService.Data;
using Intchain.UserService.DTOs;
using Intchain.UserService.Models;

namespace Intchain.UserService.Services;

/// <summary>
/// 认证服务实现
/// </summary>
public class AuthService : IAuthService
{
    private readonly UserDbContext _context;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtTokenService _jwtTokenService;

    public AuthService(
        UserDbContext context,
        IPasswordHasher passwordHasher,
        IJwtTokenService jwtTokenService)
    {
        _context = context;
        _passwordHasher = passwordHasher;
        _jwtTokenService = jwtTokenService;
    }

    /// <summary>
    /// 用户注册
    /// </summary>
    public async Task<AuthResponse?> RegisterAsync(RegisterRequest request)
    {
        // 检查用户名是否已存在
        var existingUser = await _context.Users
            .FirstOrDefaultAsync(u => u.Username == request.Username);

        if (existingUser != null)
        {
            return null;
        }

        // 创建新用户
        var user = new User
        {
            Username = request.Username,
            PasswordHash = _passwordHasher.HashPassword(request.Password),
            Role = request.Role,
            OpenId = request.OpenId,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        // 生成JWT令牌
        var token = _jwtTokenService.GenerateToken(user);

        return new AuthResponse
        {
            Token = token,
            UserId = user.Id,
            Username = user.Username,
            Role = user.Role,
            EntityId = user.EntityId
        };
    }

    /// <summary>
    /// 用户登录
    /// </summary>
    public async Task<AuthResponse?> LoginAsync(LoginRequest request)
    {
        // 查找用户
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Username == request.Username && u.IsActive);

        if (user == null)
        {
            return null;
        }

        // 验证密码
        if (!_passwordHasher.VerifyPassword(user.PasswordHash, request.Password))
        {
            return null;
        }

        // 生成JWT令牌
        var token = _jwtTokenService.GenerateToken(user);

        return new AuthResponse
        {
            Token = token,
            UserId = user.Id,
            Username = user.Username,
            Role = user.Role,
            EntityId = user.EntityId
        };
    }
}
