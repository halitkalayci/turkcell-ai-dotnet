using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using TurkcellAI.Core.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Antiforgery for state-changing endpoints (wired later on specific actions)
builder.Services.AddAntiforgery(options =>
{
    options.Cookie.Name = "bff.xsrf";
    options.Cookie.HttpOnly = false; // token readable by SPA to set header
    options.Cookie.SameSite = SameSiteMode.Lax; // Dev; use None+Secure in HTTPS
    options.HeaderName = "X-XSRF-TOKEN";
});

// AuthN: Cookie + OIDC (PKCE)
var authority = builder.Configuration["Jwt:Authority"] ?? "http://localhost:8080/realms/turkcell-ai";
var clientId = builder.Configuration["Oidc:ClientId"] ?? "bff";

builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
})
.AddCookie(options =>
{
    options.Cookie.Name = "session";
    options.Cookie.HttpOnly = true;
    options.Cookie.SameSite = SameSiteMode.Lax; // Dev; use None+Secure in HTTPS
    options.SlidingExpiration = true;
})
.AddOpenIdConnect(options =>
{
    options.Authority = authority;
    options.ClientId = clientId;
    options.ResponseType = "code";
    options.UsePkce = true;
    options.SaveTokens = false; // keep tokens server-side only if needed (e.g., distributed cache)
    options.GetClaimsFromUserInfoEndpoint = true;
    options.RequireHttpsMetadata = false; // dev only
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidIssuer = authority
    };
    options.CallbackPath = "/signin-oidc";
});

builder.Services.AddAuthorization();

var app = builder.Build();

app.UseCoreExceptionHandling();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
