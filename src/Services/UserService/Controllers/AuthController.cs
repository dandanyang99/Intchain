using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Intchain.UserService.DTOs;
using Intchain.UserService.Services;

namespace Intchain.UserService.Controllers;

/// <summary>
/// 认证控制器
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    /// <summary>
    /// 用户注册
    /// </summary>
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var result = await _authService.RegisterAsync(request);

        if (result == null)
        {
            return BadRequest(new { message = "用户名已存在" });
        }

        return Ok(result);
    }

    /// <summary>
    /// 用户登录
    /// </summary>
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var result = await _authService.LoginAsync(request);

        if (result == null)
        {
            return Unauthorized(new { message = "用户名或密码错误" });
        }

        return Ok(result);
    }

    /// <summary>
    /// 微信登录
    /// </summary>
    [HttpPost("wechat-login")]
    public async Task<IActionResult> WeChatLogin([FromBody] WeChatLoginRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var result = await _authService.WeChatLoginAsync(request);

        if (result == null)
        {
            return Unauthorized(new { message = "微信登录失败，请确保已注册并绑定微信账号" });
        }

        return Ok(result);
    }

    /// <summary>
    /// 绑定微信账号（需要认证）
    /// </summary>
    [HttpPost("bind-wechat")]
    [Authorize]
    public async Task<IActionResult> BindWeChat([FromBody] BindWeChatRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        // 从JWT token中获取用户ID
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
        {
            return Unauthorized(new { message = "无效的认证信息" });
        }

        var success = await _authService.BindWeChatAsync(userId, request.Code);

        if (!success)
        {
            return BadRequest(new { message = "绑定失败，该微信账号可能已被其他用户绑定" });
        }

        return Ok(new { message = "绑定成功" });
    }

    /// <summary>
    /// 获取微信OpenId（用于注册时绑定）
    /// </summary>
    [HttpPost("get-openid")]
    public async Task<IActionResult> GetOpenId([FromBody] GetOpenIdRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var result = await _authService.GetOpenIdAsync(request.Code);

        if (result == null)
        {
            return BadRequest(new { message = "获取OpenId失败，请检查微信登录凭证是否有效" });
        }

        return Ok(result);
    }
}
