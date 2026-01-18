using System.Text.Json;
using Intchain.UserService.DTOs;

namespace Intchain.UserService.Services;

/// <summary>
/// 微信API服务实现
/// </summary>
public class WeChatService : IWeChatService
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;
    private readonly ILogger<WeChatService> _logger;

    public WeChatService(
        HttpClient httpClient,
        IConfiguration configuration,
        ILogger<WeChatService> logger)
    {
        _httpClient = httpClient;
        _configuration = configuration;
        _logger = logger;
    }

    /// <summary>
    /// 通过code换取微信session信息
    /// </summary>
    public async Task<WeChatSessionResponse?> GetSessionByCodeAsync(string code)
    {
        try
        {
            // 获取配置
            var appId = _configuration["WeChat:AppId"];
            var appSecret = _configuration["WeChat:AppSecret"];
            var apiBaseUrl = _configuration["WeChat:ApiBaseUrl"];
            var jscode2sessionUrl = _configuration["WeChat:Jscode2sessionUrl"];

            if (string.IsNullOrEmpty(appId) || string.IsNullOrEmpty(appSecret))
            {
                _logger.LogError("WeChat configuration is missing");
                return null;
            }

            // 构建请求URL
            var requestUrl = $"{apiBaseUrl}{jscode2sessionUrl}?appid={appId}&secret={appSecret}&js_code={code}&grant_type=authorization_code";

            _logger.LogInformation("Calling WeChat API: jscode2session");

            // 调用微信API
            var response = await _httpClient.GetAsync(requestUrl);
            response.EnsureSuccessStatusCode();

            var responseContent = await response.Content.ReadAsStringAsync();
            var sessionResponse = JsonSerializer.Deserialize<WeChatSessionResponse>(responseContent);

            if (sessionResponse == null)
            {
                _logger.LogError("Failed to deserialize WeChat API response");
                return null;
            }

            // 检查微信API错误
            if (sessionResponse.ErrCode.HasValue && sessionResponse.ErrCode.Value != 0)
            {
                _logger.LogWarning("WeChat API returned error: {ErrCode} - {ErrMsg}",
                    sessionResponse.ErrCode, sessionResponse.ErrMsg);
                return sessionResponse;
            }

            _logger.LogInformation("Successfully retrieved WeChat session for OpenId: {OpenId}",
                MaskOpenId(sessionResponse.OpenId));

            return sessionResponse;
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "HTTP request to WeChat API failed");
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error calling WeChat API");
            return null;
        }
    }

    /// <summary>
    /// 遮蔽OpenId用于日志记录
    /// </summary>
    private string MaskOpenId(string openId)
    {
        if (string.IsNullOrEmpty(openId) || openId.Length <= 8)
        {
            return "****";
        }
        return $"{openId.Substring(0, 4)}****{openId.Substring(openId.Length - 4)}";
    }
}
