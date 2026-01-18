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
    private readonly IWeChatService _weChatService;

    public AuthService(
        UserDbContext context,
        IPasswordHasher passwordHasher,
        IJwtTokenService jwtTokenService,
        IWeChatService weChatService)
    {
        _context = context;
        _passwordHasher = passwordHasher;
        _jwtTokenService = jwtTokenService;
        _weChatService = weChatService;
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

    /// <summary>
    /// 微信登录
    /// </summary>
    public async Task<AuthResponse?> WeChatLoginAsync(WeChatLoginRequest request)
    {
        // 1. 调用微信API获取session信息
        var wechatSession = await _weChatService.GetSessionByCodeAsync(request.Code);

        // 2. 检查微信API调用是否成功
        if (wechatSession == null || wechatSession.ErrCode.HasValue)
        {
            return null;
        }

        // 3. 根据OpenId查找用户
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.OpenId == wechatSession.OpenId && u.IsActive);

        // 4. 如果用户不存在，返回null（要求用户必须先注册）
        if (user == null)
        {
            return null;
        }

        // 5. 更新SessionKey和UnionId（如果有变化）
        bool needUpdate = false;
        if (user.SessionKey != wechatSession.SessionKey)
        {
            user.SessionKey = wechatSession.SessionKey;
            needUpdate = true;
        }
        if (!string.IsNullOrEmpty(wechatSession.UnionId) && user.UnionId != wechatSession.UnionId)
        {
            user.UnionId = wechatSession.UnionId;
            needUpdate = true;
        }
        if (needUpdate)
        {
            user.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
        }

        // 6. 生成JWT令牌
        var token = _jwtTokenService.GenerateToken(user);

        // 7. 返回认证响应
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
    /// 绑定微信账号
    /// </summary>
    public async Task<bool> BindWeChatAsync(int userId, string code)
    {
        // 1. 调用微信API获取session信息
        var wechatSession = await _weChatService.GetSessionByCodeAsync(code);

        if (wechatSession == null || wechatSession.ErrCode.HasValue)
        {
            return false;
        }

        // 2. 检查OpenId是否已被其他用户绑定
        var existingUser = await _context.Users
            .FirstOrDefaultAsync(u => u.OpenId == wechatSession.OpenId && u.Id != userId);

        if (existingUser != null)
        {
            // OpenId已被其他用户绑定
            return false;
        }

        // 3. 查找当前用户
        var user = await _context.Users.FindAsync(userId);
        if (user == null)
        {
            return false;
        }

        // 4. 绑定微信信息
        user.OpenId = wechatSession.OpenId;
        user.SessionKey = wechatSession.SessionKey;
        user.UnionId = wechatSession.UnionId;
        user.OpenIdBoundAt = DateTime.UtcNow;
        user.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return true;
    }

    /// <summary>
    /// 获取微信OpenId（用于注册时绑定）
    /// </summary>
    public async Task<GetOpenIdResponse?> GetOpenIdAsync(string code)
    {
        // 1. 调用微信API获取session信息
        var wechatSession = await _weChatService.GetSessionByCodeAsync(code);

        if (wechatSession == null || wechatSession.ErrCode.HasValue)
        {
            return null;
        }

        // 2. 检查OpenId是否已被其他用户绑定
        var existingUser = await _context.Users
            .FirstOrDefaultAsync(u => u.OpenId == wechatSession.OpenId);

        // 3. 返回OpenId和绑定状态
        return new GetOpenIdResponse
        {
            OpenId = wechatSession.OpenId,
            UnionId = wechatSession.UnionId,
            IsAlreadyBound = existingUser != null
        };
    }
}
