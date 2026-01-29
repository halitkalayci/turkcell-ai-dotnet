using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TurkcellAI.Core.Application.DTOs;
using TurkcellAI.Core.Application.Enums;

namespace Identity.Bff.Controllers;

[ApiController]
public class AuthController : ControllerBase
{
    [HttpGet("/bff/login/start")]
    [AllowAnonymous]
    public IActionResult LoginStart([FromQuery] string? returnUrl = "/")
    {
        return Challenge(new AuthenticationProperties { RedirectUri = returnUrl ?? "/" }, "OpenIdConnect");
    }

    // OpenIdConnect handler processes /signin-oidc automatically

    [HttpPost("/bff/logout")]
    [Authorize]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return NoContent();
    }
}
