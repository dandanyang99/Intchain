using Intchain.UserService.Data;
using Intchain.UserService.DTOs;
using Intchain.UserService.Models;
using Intchain.UserService.Services;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace Intchain.UserService.Tests.Services;

/// <summary>
/// AuthService单元测试
/// </summary>
public class AuthServiceTests
{
    private readonly Mock<IWeChatService> _mockWeChatService;
    private readonly Mock<IPasswordHasher> _mockPasswordHasher;
    private readonly Mock<IJwtTokenService> _mockJwtTokenService;
    private readonly UserDbContext _context;
    private readonly AuthService _authService;

    public AuthServiceTests()
    {
        // 创建内存数据库
        var options = new DbContextOptionsBuilder<UserDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new UserDbContext(options);

        // 创建Mock对象
        _mockWeChatService = new Mock<IWeChatService>();
        _mockPasswordHasher = new Mock<IPasswordHasher>();
        _mockJwtTokenService = new Mock<IJwtTokenService>();

        // 创建AuthService实例
        _authService = new AuthService(
            _context,
            _mockPasswordHasher.Object,
            _mockJwtTokenService.Object,
            _mockWeChatService.Object
        );
    }

    /// <summary>
    /// 测试：当微信API返回有效OpenId且未被绑定时，应返回IsAlreadyBound=false
    /// </summary>
    [Fact]
    public async Task GetOpenIdAsync_WhenOpenIdNotBound_ReturnsResponseWithIsAlreadyBoundFalse()
    {
        // Arrange
        var code = "valid_code";
        var expectedOpenId = "test_openid_123";
        var expectedUnionId = "test_unionid_456";

        // 模拟微信API返回成功响应
        _mockWeChatService
            .Setup(x => x.GetSessionByCodeAsync(code))
            .ReturnsAsync(new WeChatSessionResponse
            {
                OpenId = expectedOpenId,
                UnionId = expectedUnionId,
                SessionKey = "test_session_key",
                ErrCode = null,
                ErrMsg = null
            });

        // Act
        var result = await _authService.GetOpenIdAsync(code);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedOpenId, result.OpenId);
        Assert.Equal(expectedUnionId, result.UnionId);
        Assert.False(result.IsAlreadyBound);
    }

    /// <summary>
    /// 测试：当微信API返回有效OpenId但已被其他用户绑定时，应返回IsAlreadyBound=true
    /// </summary>
    [Fact]
    public async Task GetOpenIdAsync_WhenOpenIdAlreadyBound_ReturnsResponseWithIsAlreadyBoundTrue()
    {
        // Arrange
        var code = "valid_code";
        var existingOpenId = "existing_openid_123";
        var expectedUnionId = "test_unionid_456";

        // 在数据库中创建一个已绑定该OpenId的用户
        var existingUser = new User
        {
            Username = "existing_user",
            PasswordHash = "hashed_password",
            Role = "SalesOutlet",
            OpenId = existingOpenId
        };
        _context.Users.Add(existingUser);
        await _context.SaveChangesAsync();

        // 模拟微信API返回成功响应
        _mockWeChatService
            .Setup(x => x.GetSessionByCodeAsync(code))
            .ReturnsAsync(new WeChatSessionResponse
            {
                OpenId = existingOpenId,
                UnionId = expectedUnionId,
                SessionKey = "test_session_key",
                ErrCode = null,
                ErrMsg = null
            });

        // Act
        var result = await _authService.GetOpenIdAsync(code);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(existingOpenId, result.OpenId);
        Assert.Equal(expectedUnionId, result.UnionId);
        Assert.True(result.IsAlreadyBound);
    }

    /// <summary>
    /// 测试：当微信API返回错误时，应返回null
    /// </summary>
    [Fact]
    public async Task GetOpenIdAsync_WhenWeChatApiReturnsError_ReturnsNull()
    {
        // Arrange
        var code = "invalid_code";

        // 模拟微信API返回错误响应
        _mockWeChatService
            .Setup(x => x.GetSessionByCodeAsync(code))
            .ReturnsAsync(new WeChatSessionResponse
            {
                OpenId = null,
                UnionId = null,
                SessionKey = null,
                ErrCode = 40029,
                ErrMsg = "invalid code"
            });

        // Act
        var result = await _authService.GetOpenIdAsync(code);

        // Assert
        Assert.Null(result);
    }

    /// <summary>
    /// 测试：当微信API返回null时，应返回null
    /// </summary>
    [Fact]
    public async Task GetOpenIdAsync_WhenWeChatApiReturnsNull_ReturnsNull()
    {
        // Arrange
        var code = "invalid_code";

        // 模拟微信API返回null
        _mockWeChatService
            .Setup(x => x.GetSessionByCodeAsync(code))
            .ReturnsAsync((WeChatSessionResponse?)null);

        // Act
        var result = await _authService.GetOpenIdAsync(code);

        // Assert
        Assert.Null(result);
    }

    /// <summary>
    /// 测试：成功注册并绑定OpenId
    /// </summary>
    [Fact]
    public async Task RegisterAsync_WithOpenId_ReturnsAuthResponseWithOpenIdBound()
    {
        // Arrange
        var request = new RegisterRequest
        {
            Username = "newuser",
            Password = "password123",
            Role = "SalesOutlet",
            OpenId = "new_openid_123"
        };

        _mockPasswordHasher
            .Setup(x => x.HashPassword(request.Password))
            .Returns("hashed_password");

        _mockJwtTokenService
            .Setup(x => x.GenerateToken(It.IsAny<User>()))
            .Returns("jwt_token_123");

        // Act
        var result = await _authService.RegisterAsync(request);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("jwt_token_123", result.Token);
        Assert.Equal(request.Username, result.Username);
        Assert.Equal(request.Role, result.Role);

        // 验证用户已保存到数据库
        var savedUser = await _context.Users.FirstOrDefaultAsync(u => u.Username == request.Username);
        Assert.NotNull(savedUser);
        Assert.Equal(request.OpenId, savedUser.OpenId);
    }

    /// <summary>
    /// 测试：成功注册但不绑定OpenId
    /// </summary>
    [Fact]
    public async Task RegisterAsync_WithoutOpenId_ReturnsAuthResponseWithoutOpenIdBound()
    {
        // Arrange
        var request = new RegisterRequest
        {
            Username = "newuser2",
            Password = "password123",
            Role = "SalesOutlet",
            OpenId = null
        };

        _mockPasswordHasher
            .Setup(x => x.HashPassword(request.Password))
            .Returns("hashed_password");

        _mockJwtTokenService
            .Setup(x => x.GenerateToken(It.IsAny<User>()))
            .Returns("jwt_token_456");

        // Act
        var result = await _authService.RegisterAsync(request);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("jwt_token_456", result.Token);
        Assert.Equal(request.Username, result.Username);

        // 验证用户已保存到数据库且OpenId为null
        var savedUser = await _context.Users.FirstOrDefaultAsync(u => u.Username == request.Username);
        Assert.NotNull(savedUser);
        Assert.Null(savedUser.OpenId);
    }

    /// <summary>
    /// 测试：当用户名已存在时，注册失败返回null
    /// </summary>
    [Fact]
    public async Task RegisterAsync_WhenUsernameExists_ReturnsNull()
    {
        // Arrange
        // 先创建一个已存在的用户
        var existingUser = new User
        {
            Username = "existinguser",
            PasswordHash = "hashed_password",
            Role = "SalesOutlet"
        };
        _context.Users.Add(existingUser);
        await _context.SaveChangesAsync();

        // 尝试用相同的用户名注册
        var request = new RegisterRequest
        {
            Username = "existinguser",
            Password = "password123",
            Role = "LotteryCenter"
        };

        // Act
        var result = await _authService.RegisterAsync(request);

        // Assert
        Assert.Null(result);
    }
}
