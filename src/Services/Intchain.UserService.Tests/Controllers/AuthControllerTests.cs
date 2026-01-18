using Intchain.UserService.Controllers;
using Intchain.UserService.DTOs;
using Intchain.UserService.Services;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace Intchain.UserService.Tests.Controllers;

/// <summary>
/// AuthController单元测试
/// </summary>
public class AuthControllerTests
{
    private readonly Mock<IAuthService> _mockAuthService;
    private readonly AuthController _controller;

    public AuthControllerTests()
    {
        _mockAuthService = new Mock<IAuthService>();
        _controller = new AuthController(_mockAuthService.Object);
    }

    /// <summary>
    /// 测试：当请求有效且OpenId未被绑定时，应返回200 OK和OpenId信息
    /// </summary>
    [Fact]
    public async Task GetOpenId_WhenRequestValidAndOpenIdNotBound_ReturnsOkWithOpenIdInfo()
    {
        // Arrange
        var request = new GetOpenIdRequest { Code = "valid_code" };
        var expectedResponse = new GetOpenIdResponse
        {
            OpenId = "test_openid_123",
            UnionId = "test_unionid_456",
            IsAlreadyBound = false
        };

        _mockAuthService
            .Setup(x => x.GetOpenIdAsync(request.Code))
            .ReturnsAsync(expectedResponse);

        // Act
        var result = await _controller.GetOpenId(request);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var response = Assert.IsType<GetOpenIdResponse>(okResult.Value);
        Assert.Equal(expectedResponse.OpenId, response.OpenId);
        Assert.Equal(expectedResponse.UnionId, response.UnionId);
        Assert.False(response.IsAlreadyBound);
    }

    /// <summary>
    /// 测试：当OpenId已被绑定时，应返回200 OK和IsAlreadyBound=true
    /// </summary>
    [Fact]
    public async Task GetOpenId_WhenOpenIdAlreadyBound_ReturnsOkWithIsAlreadyBoundTrue()
    {
        // Arrange
        var request = new GetOpenIdRequest { Code = "valid_code" };
        var expectedResponse = new GetOpenIdResponse
        {
            OpenId = "test_openid_123",
            UnionId = "test_unionid_456",
            IsAlreadyBound = true
        };

        _mockAuthService
            .Setup(x => x.GetOpenIdAsync(request.Code))
            .ReturnsAsync(expectedResponse);

        // Act
        var result = await _controller.GetOpenId(request);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var response = Assert.IsType<GetOpenIdResponse>(okResult.Value);
        Assert.Equal(expectedResponse.OpenId, response.OpenId);
        Assert.True(response.IsAlreadyBound);
    }

    /// <summary>
    /// 测试：当服务返回null时，应返回400 BadRequest
    /// </summary>
    [Fact]
    public async Task GetOpenId_WhenServiceReturnsNull_ReturnsBadRequest()
    {
        // Arrange
        var request = new GetOpenIdRequest { Code = "invalid_code" };

        _mockAuthService
            .Setup(x => x.GetOpenIdAsync(request.Code))
            .ReturnsAsync((GetOpenIdResponse?)null);

        // Act
        var result = await _controller.GetOpenId(request);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.NotNull(badRequestResult.Value);
    }

    /// <summary>
    /// 测试：当ModelState无效时，应返回400 BadRequest
    /// </summary>
    [Fact]
    public async Task GetOpenId_WhenModelStateInvalid_ReturnsBadRequest()
    {
        // Arrange
        var request = new GetOpenIdRequest { Code = "" };
        _controller.ModelState.AddModelError("Code", "微信登录凭证不能为空");

        // Act
        var result = await _controller.GetOpenId(request);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.IsType<SerializableError>(badRequestResult.Value);
    }
}
