using Microsoft.AspNetCore.Mvc;
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
}
