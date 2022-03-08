using System.Globalization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Localization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

// Configure the request localization feature
// app.UseRequestLocalization(options =>
// {
//     var allCultures = new List<CultureInfo> {new("hr-hr"), new("is-is"), new("en-us")};
//
//     options.SupportedCultures = allCultures;
//     options.SupportedUICultures = allCultures;
// });

// Configure the middleware we'll be using to set the culture for the current execution context
app.Use(async (context, next) =>
{
    var tenantId = context.Request.Cookies["tenantId"];
    if (tenantId is null)
    {
        await next(context);
        return;
    }

    var countryId = Math.Max(int.Parse(tenantId), 1);
    var currencyService = new CurrencyService();
    var currencyFormat = currencyService.GetCurrencyForCountry(countryId);

    CultureInfo.DefaultThreadCurrentCulture = CultureInfo.DefaultThreadCurrentUICulture = new CultureInfo(currencyFormat.Locale);

    var numberFormat = CultureInfo.DefaultThreadCurrentCulture!.NumberFormat;
    numberFormat.CurrencySymbol = currencyFormat.CurrencySign;
    numberFormat.CurrencyDecimalSeparator = currencyFormat.DecimalSeparator;
    numberFormat.CurrencyGroupSeparator = currencyFormat.GroupSeparator;
    numberFormat.CurrencyDecimalDigits = currencyFormat.DecimalPrecision;

    await next(context);
});

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();

internal sealed class CurrencyService
{
    private static readonly IList<CurrencyFormatDto> CurrencyFormats = new List<CurrencyFormatDto>
    {
        new("ISK", ",", ".", 0, "is-is"),
        new("kn", ".", ",", 2, "hr-hr"),
        new("$", ".", ",", 2, "en-us")
    };

    public CurrencyFormatDto GetCurrencyForCountry(int countryId)
    {
        return CurrencyFormats[countryId - 1];
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