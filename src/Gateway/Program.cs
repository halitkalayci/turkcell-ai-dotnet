using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using System.Threading.RateLimiting;
using TurkcellAI.Gateway.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Load YARP routes from external yarp.json as part of configuration
builder.Configuration.AddJsonFile("yarp.json", optional: false, reloadOnChange: true);

// Config bindings
builder.Services.Configure<RequestSecurityOptions>(builder.Configuration.GetSection("Security"));

// Health
builder.Services.AddRouting();
builder.Services.AddHealthChecks();

// Rate limiting (simple IP-based concurrency + token bucket)
builder.Services.AddRateLimiter(options =>
{
	options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
	options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(context =>
	{
		var key = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
		return RateLimitPartition.GetTokenBucketLimiter(key, _ => new TokenBucketRateLimiterOptions
		{
			TokenLimit = 100,
			QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
			ReplenishmentPeriod = TimeSpan.FromSeconds(10),
			TokensPerPeriod = 20,
			AutoReplenishment = true,
			QueueLimit = 50
		});
	});
});

// Authentication (JWT via Keycloak)
var authority = builder.Configuration["Jwt:Authority"] ?? "http://localhost:8080/realms/turkcell-ai";
var validAudiences = builder.Configuration.GetSection("Jwt:ValidAudiences").Get<string[]>() ?? new[] { "gateway" };
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
	.AddJwtBearer(options =>
	{
		options.Authority = authority;
		options.RequireHttpsMetadata = false; // dev only; enable in prod
		options.TokenValidationParameters = new TokenValidationParameters
		{
			ValidateIssuer = true,
			ValidIssuer = authority,
			ValidateAudience = true,
			ValidAudiences = validAudiences,
			ValidateLifetime = true,
			ClockSkew = TimeSpan.FromMinutes(2)
		};
		options.Events = new JwtBearerEvents
		{
			OnAuthenticationFailed = ctx =>
			{
				ctx.HttpContext.Response.Headers["WWW-Authenticate"] = "Bearer error=invalid_token";
				return Task.CompletedTask;
			}
		};
	});

builder.Services.AddAuthorization();

// YARP
builder.Services.AddReverseProxy()
	.LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

// Observability excluded in this implementation scope

var app = builder.Build();

app.UseRateLimiter();
app.UseMiddleware<RequestSecurityMiddleware>();
app.UseAuthentication();
app.UseAuthorization();

app.MapHealthChecks("/health");
app.MapGet("/", () => Results.Ok(new { name = "TurkcellAI Gateway", status = "ok" }));

// Deny-by-default for anything not matched by YARP routes unless explicitly allowed
app.MapReverseProxy();

app.Run();
