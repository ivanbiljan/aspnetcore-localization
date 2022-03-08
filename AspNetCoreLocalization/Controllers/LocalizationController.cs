using System.Globalization;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;

namespace AspNetCoreLocalization.Controllers;

[ApiController]
[Route("api/localization")]
public sealed class LocalizationController : ControllerBase
{
    public async Task<IActionResult> SetActiveLanguage(int tenantId)
    {
        Response.Cookies.Append("marketId",
            tenantId.ToString(),
            new CookieOptions {Expires = DateTimeOffset.UtcNow.AddYears(1)});

        return RedirectToAction("Index", "Home");
    }
}