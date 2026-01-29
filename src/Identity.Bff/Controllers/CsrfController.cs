using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Identity.Bff.Controllers;

[ApiController]
public class CsrfController : ControllerBase
{
    private readonly IAntiforgery _antiforgery;
    public CsrfController(IAntiforgery antiforgery) => _antiforgery = antiforgery;

    [HttpGet("/bff/csrf")]
    [AllowAnonymous]
    public IActionResult GetToken()
    {
        var tokens = _antiforgery.GetAndStoreTokens(HttpContext);
        return Ok(new { token = tokens.RequestToken });
    }
}
