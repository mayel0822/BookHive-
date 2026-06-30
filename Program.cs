using BookHive.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Web;

var builder = WebApplication.CreateBuilder(args);

// ── Add Controllers ─────────────────────────
builder.Services.AddControllersWithViews();

// ── Add Razor Pages (needed for Identity UI) 
builder.Services.AddRazorPages();

// ── Add Database ────────────────────────────
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("BookHiveDB")
    )
);

// ── Add Session ─────────────────────────────
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// ── Add Microsoft Identity ──────────────────
builder.Services.AddMicrosoftIdentityWebAppAuthentication(
    builder.Configuration, "AzureAd")
    .EnableTokenAcquisitionToCallDownstreamApi()
    .AddInMemoryTokenCaches();

builder.Services.Configure<Microsoft.AspNetCore.Authentication.OpenIdConnect.OpenIdConnectOptions>(
    "OpenIdConnect", options =>
    {
        options.Events.OnRedirectToIdentityProvider = context =>
        {
            if (context.Properties.Items.TryGetValue("prompt", out var prompt))
                context.ProtocolMessage.Prompt = prompt;
            return Task.CompletedTask;
        };
    });

var app = builder.Build();

// ── Configure Pipeline ──────────────────────
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseStaticFiles();
app.UseRouting();
app.UseSession();
app.UseAuthentication();
app.UseAuthorization();
app.MapStaticAssets();
app.MapRazorPages();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();