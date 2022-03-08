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
        var currency = Currency.DefaultCurrencies[tenantId % Currency.DefaultCurrencies.Count];
        var culture = new CultureInfo(currency.Locale);
        
        Response.Cookies.Append("tenantId",
            tenantId.ToString(),
            new CookieOptions {Expires = DateTimeOffset.UtcNow.AddYears(1)});

        return RedirectToAction("Index", "Home");
    }
}