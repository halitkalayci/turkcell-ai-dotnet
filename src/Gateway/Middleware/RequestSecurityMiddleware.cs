using System.Net;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace TurkcellAI.Gateway.Middleware;

public sealed class RequestSecurityOptions
{
    public string[] IpAllowList { get; set; } = ["::1"]; // '*' means allow all
    public string[] IpDenyList { get; set; } = [];
    public string CorrelationHeaderName { get; set; } = "X-Correlation-ID";
}

public class RequestSecurityMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestSecurityMiddleware> _logger;
    private readonly RequestSecurityOptions _options;

    public RequestSecurityMiddleware(RequestDelegate next, ILogger<RequestSecurityMiddleware> logger, IOptions<RequestSecurityOptions> options)
    {
        _next = next;
        _logger = logger;
        _options = options.Value;
    }

    public async Task Invoke(HttpContext context)
    {
        // IP allow/deny evaluation
        var remoteIp = context.Connection.RemoteIpAddress;
        if (remoteIp is not null)
        {
            if (IsDenied(remoteIp))
            {
                _logger.LogWarning("Request denied by IP deny list: {RemoteIp}", remoteIp);
                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                await context.Response.WriteAsync("Forbidden");
                return;
            }

            if (!IsAllowed(remoteIp))
            {
                _logger.LogWarning("Request not in IP allow list: {RemoteIp}", remoteIp);
                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                await context.Response.WriteAsync("Forbidden");
                return;
            }
        }

        // Correlation ID propagation
        var headerName = _options.CorrelationHeaderName;
        if (!context.Request.Headers.TryGetValue(headerName, out var correlationId) || string.IsNullOrWhiteSpace(correlationId))
        {
            correlationId = Guid.NewGuid().ToString();
            context.Request.Headers[headerName] = correlationId;
        }

        using (_logger.BeginScope(new Dictionary<string, object>
        {
            ["correlationId"] = correlationId.ToString(),
            ["clientIp"] = remoteIp?.ToString() ?? "unknown",
            ["userId"] = context.User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "anonymous"
        }))
        {
            await _next(context);
        }
    }

    private bool IsDenied(IPAddress ip)
    {
        return _options.IpDenyList.Any(range => IpMatches(ip, range));
    }

    private bool IsAllowed(IPAddress ip)
    {
        // '*' allows all
        if (_options.IpAllowList.Length == 1 && _options.IpAllowList[0] == "*") return true;
        return _options.IpAllowList.Any(range => IpMatches(ip, range));
    }

    private static bool IpMatches(IPAddress ip, string cidrOrIp)
    {
        // Very simple matcher: supports exact IP or CIDR (IPv4)
        if (cidrOrIp.Contains('/'))
        {
            var parts = cidrOrIp.Split('/');
            if (parts.Length != 2) return false;
            if (!IPAddress.TryParse(parts[0], out var baseIp)) return false;
            if (!int.TryParse(parts[1], out var prefix)) return false;
            if (ip.AddressFamily != baseIp.AddressFamily) return false;

            var ipBytes = ip.GetAddressBytes();
            var baseBytes = baseIp.GetAddressBytes();
            int fullBytes = prefix / 8;
            int remainderBits = prefix % 8;

            for (int i = 0; i < fullBytes; i++)
            {
                if (ipBytes[i] != baseBytes[i]) return false;
            }
            if (remainderBits == 0) return true;

            int mask = (byte)~(0xFF >> remainderBits);
            return (ipBytes[fullBytes] & mask) == (baseBytes[fullBytes] & mask);
        }
        else
        {
            return IPAddress.TryParse(cidrOrIp, out var exact) && exact.Equals(ip);
        }
    }
}
