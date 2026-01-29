using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Identity.Bff.Controllers;

[ApiController]
public class UserController : ControllerBase
{
    [HttpGet("/bff/user")]
    [Authorize]
    public IActionResult GetUser()
    {
        var subject = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirst("sub")?.Value ?? "unknown";
        var permissions = User.FindAll("permissions").Select(c => c.Value).ToArray();
        var name = User.FindFirstValue("name");
        var email = User.FindFirstValue("email");

        return Ok(new
        {
            subject,
            permissions,
            name,
            email
        });
    }
}
