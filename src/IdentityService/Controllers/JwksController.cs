using Microsoft.AspNetCore.Mvc;
using TurkcellAI.Core.Application.DTOs;
using TurkcellAI.Core.Application.Enums;

namespace IdentityService.Controllers;

[ApiController]
public class JwksController : ControllerBase
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly string _authority;

    public JwksController(IHttpClientFactory httpClientFactory, IConfiguration configuration)
    {
        _httpClientFactory = httpClientFactory;
        _authority = configuration["Jwt:Authority"] ?? "http://localhost:8080/realms/turkcell-ai";
    }

    [HttpGet("oauth/public-keys")]
    public async Task<IActionResult> GetPublicKeys()
    {
        var jwks = _authority.TrimEnd('/') + "/protocol/openid-connect/certs";
        var client = _httpClientFactory.CreateClient("Idp");
        var resp = await client.GetAsync(jwks);
        var body = await resp.Content.ReadAsStringAsync();
        return StatusCode((int)resp.StatusCode, TryParseJson(body));
    }

    [HttpGet("userinfo")]
    public async Task<IActionResult> UserInfo()
    {
        if (!Request.Headers.TryGetValue("Authorization", out var auth) || string.IsNullOrWhiteSpace(auth))
        {
            return Unauthorized(new ErrorResponse
            {
                TraceId = HttpContext.TraceIdentifier,
                Code = ErrorCode.UNAUTHORIZED,
                Message = "Missing Authorization header"
            });
        }

        var userinfo = _authority.TrimEnd('/') + "/protocol/openid-connect/userinfo";
        var client = _httpClientFactory.CreateClient("Idp");
        var req = new HttpRequestMessage(HttpMethod.Get, userinfo);
        req.Headers.TryAddWithoutValidation("Authorization", auth.ToString());
        var resp = await client.SendAsync(req);
        var body = await resp.Content.ReadAsStringAsync();
        return StatusCode((int)resp.StatusCode, TryParseJson(body));
    }

    private static object TryParseJson(string text)
    {
        try { return System.Text.Json.JsonSerializer.Deserialize<object>(text) ?? text; }
        catch { return text; }
    }
}
