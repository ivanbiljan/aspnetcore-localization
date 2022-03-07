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
app.UseRequestLocalization(options =>
{
    var allCultures = new List<CultureInfo> {new("hr-hr"), new("is-is"), new("en-us")};

    options.SupportedCultures = allCultures;
    options.SupportedUICultures = allCultures;
});

// Configure the middleware we'll be using to set the culture for the current execution context
app.Use(async (context, next) =>
{
    var culture = context.Features.Get<IRequestCultureFeature>()!.RequestCulture.Culture;

    CultureInfo.DefaultThreadCurrentCulture = culture;
    CultureInfo.DefaultThreadCurrentUICulture = culture;

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