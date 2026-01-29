using System.Net.Http.Headers;
using Microsoft.AspNetCore.Mvc;
using TurkcellAI.Core.Application.DTOs;
using TurkcellAI.Core.Application.Enums;

namespace IdentityService.Controllers;

[ApiController]
[Route("oauth")]
public class OAuthController : ControllerBase
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly string _authority;

    public OAuthController(IHttpClientFactory httpClientFactory, IConfiguration configuration)
    {
        _httpClientFactory = httpClientFactory;
        _authority = configuration["Jwt:Authority"] ?? "http://localhost:8080/realms/turkcell-ai";
    }

    [HttpPost("token")]
    [Consumes("application/x-www-form-urlencoded")]
    public async Task<IActionResult> Token()
    {
        if (!Request.HasFormContentType)
        {
            return BadRequest(new ErrorResponse
            {
                TraceId = HttpContext.TraceIdentifier,
                Code = ErrorCode.INVALID_PARAMETER,
                Message = "Content-Type must be application/x-www-form-urlencoded"
            });
        }

        var tokenEndpoint = _authority.TrimEnd('/') + "/protocol/openid-connect/token";
        var form = await Request.ReadFormAsync();
        var content = new FormUrlEncodedContent(form.ToDictionary(kv => kv.Key, kv => kv.Value.ToString()));

        var client = _httpClientFactory.CreateClient("Idp");
        using var resp = await client.PostAsync(tokenEndpoint, content);
        var body = await resp.Content.ReadAsStringAsync();
        return StatusCode((int)resp.StatusCode, TryParseJson(body));
    }

    [HttpPost("token-exchange")]
    [Consumes("application/x-www-form-urlencoded")]
    public async Task<IActionResult> TokenExchange()
    {
        if (!Request.HasFormContentType)
        {
            return BadRequest(new ErrorResponse
            {
                TraceId = HttpContext.TraceIdentifier,
                Code = ErrorCode.INVALID_PARAMETER,
                Message = "Content-Type must be application/x-www-form-urlencoded"
            });
        }

        var tokenEndpoint = _authority.TrimEnd('/') + "/protocol/openid-connect/token";
        var form = await Request.ReadFormAsync();
        var content = new FormUrlEncodedContent(form.ToDictionary(kv => kv.Key, kv => kv.Value.ToString()));

        var client = _httpClientFactory.CreateClient("Idp");
        using var resp = await client.PostAsync(tokenEndpoint, content);
        var body = await resp.Content.ReadAsStringAsync();
        return StatusCode((int)resp.StatusCode, TryParseJson(body));
    }

    private static object TryParseJson(string text)
    {
        try { return System.Text.Json.JsonSerializer.Deserialize<object>(text) ?? text; }
        catch { return text; }
    }
}
