using System.Globalization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var services = builder.Services;
services.AddControllersWithViews();
services.AddTransient<MarketService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

// Configure the middleware we'll be using to set the culture for the current execution context
app.UseMiddleware<LocalizationMiddleware>();

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();

internal sealed class LocalizationMiddleware
{
    private readonly MarketService _marketService;
    private readonly RequestDelegate _next;

    public LocalizationMiddleware(MarketService marketService, RequestDelegate next)
    {
        _marketService = marketService;
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var marketIdCookieValue = context.Request.Cookies["marketId"];
        if (!int.TryParse(marketIdCookieValue, out var marketId))
        {
            await _next(context);
            return;
        }
        
        var marketService = new MarketService();
        var currencyFormat = marketService.GetCurrencyForMarket(marketId);

        CultureInfo.DefaultThreadCurrentCulture = CultureInfo.DefaultThreadCurrentUICulture = new CultureInfo(currencyFormat.Locale);

        var numberFormat = CultureInfo.DefaultThreadCurrentCulture!.NumberFormat;
        numberFormat.CurrencySymbol = currencyFormat.CurrencySign;
        numberFormat.CurrencyDecimalSeparator = currencyFormat.DecimalSeparator;
        numberFormat.CurrencyGroupSeparator = currencyFormat.GroupSeparator;
        numberFormat.CurrencyDecimalDigits = currencyFormat.DecimalPrecision;

        await _next(context);
    }
}

internal sealed class MarketService
{
    private static readonly IList<CurrencyFormatDto> CurrencyFormats = new List<CurrencyFormatDto>
    {
        new("ISK", ",", ".", 0, "is-is"),
        new("kn", ".", ",", 2, "hr-hr"),
        new("$", ".", ",", 2, "en-us")
    };

    public CurrencyFormatDto GetCurrencyForMarket(int marketId)
    {
        return CurrencyFormats[marketId - 1];
    }
}

internal sealed class CurrencyFormatDto
{
    public CurrencyFormatDto(string currencySign, string decimalSeparator, string groupSeparator, int decimalPrecision, string locale)
    {
        CurrencySign = currencySign;
        DecimalSeparator = decimalSeparator;
        GroupSeparator = groupSeparator;
        DecimalPrecision = decimalPrecision;
        Locale = locale;
    }

    public string CurrencySign { get; set; }
	        
    public string DecimalSeparator { get; set; }
	        
    public string GroupSeparator { get; set; }
	        
    public int DecimalPrecision { get; set; }
    
    public string Locale { get; set; }
}