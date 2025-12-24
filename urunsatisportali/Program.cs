using Microsoft.EntityFrameworkCore;
using urunsatisportali.Data;
using System.Globalization;
using Microsoft.AspNetCore.Localization;
using Microsoft.Net.Http.Headers;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Identity;
using urunsatisportali.Models;
using urunsatisportali.Repositories;

var builder = WebApplication.CreateBuilder(args);


// Add services to the container.
#if DEBUG
// Add Razor Runtime Compilation for development (allows view changes without restart)
builder.Services.AddControllersWithViews().AddRazorRuntimeCompilation();
#else
builder.Services.AddControllersWithViews();
#endif

// Localization options
builder.Services.Configure<RequestLocalizationOptions>(options =>
{
    var supportedCultures = new[] { new CultureInfo("tr-TR"), new CultureInfo("en-US") };
    options.DefaultRequestCulture = new RequestCulture("tr-TR");
    options.SupportedCultures = supportedCultures;
    options.SupportedUICultures = supportedCultures;
});

// Cookie policy (can be expanded if using Auth cookies)
builder.Services.Configure<CookiePolicyOptions>(options =>
{
    options.MinimumSameSitePolicy = Microsoft.AspNetCore.Http.SameSiteMode.Lax;
    options.Secure = CookieSecurePolicy.Always;
});

// Add Entity Framework
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add ASP.NET Core Identity
builder.Services
    .AddIdentity<ApplicationUser, IdentityRole>(options =>
    {
        options.Password.RequireDigit = false;
        options.Password.RequireNonAlphanumeric = false;
        options.Password.RequireUppercase = false;
        options.Password.RequireLowercase = true;
        options.Password.RequiredLength = 6;
        options.SignIn.RequireConfirmedAccount = false;
    })
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Auth/Login";
    options.LogoutPath = "/Auth/Logout";
    options.AccessDeniedPath = "/Auth/Login";
    options.Cookie.SameSite = Microsoft.AspNetCore.Http.SameSiteMode.Strict;
    options.Cookie.HttpOnly = true;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    options.SlidingExpiration = true;
    options.ExpireTimeSpan = TimeSpan.FromDays(7);
});

// Add Generic Repository
builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));

// Add Services
builder.Services.AddScoped<urunsatisportali.Services.ISaleService, urunsatisportali.Services.SaleService>();

builder.Services.AddSignalR();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromDays(7); // 7 days persistence
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    options.Cookie.Name = ".UrunSatis.Session";
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    options.Cookie.SameSite = Microsoft.AspNetCore.Http.SameSiteMode.Lax;
});

var app = builder.Build();

// Apply pending migrations to ensure database is up to date
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<ApplicationDbContext>();
        await context.Database.MigrateAsync();

        // Seed Roles
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
        string[] roleNames = { "Owner", "Admin", "User" };

        foreach (var roleName in roleNames)
        {
            if (!await roleManager.RoleExistsAsync(roleName))
            {
                await roleManager.CreateAsync(new IdentityRole(roleName));
            }
        }

        var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();

        // Seed Owner User
        var ownerUser = await userManager.FindByNameAsync("owner");
        if (ownerUser == null)
        {
            ownerUser = new ApplicationUser
            {
                UserName = "owner",
                Email = "owner@example.com",
                EmailConfirmed = true,
                FullName = "Platform Sahibi",
                IsAdmin = true, // Keep for backward compat if needed, or rely on Role
                CreatedAt = DateTime.Now
            };
            var result = await userManager.CreateAsync(ownerUser, "Owner123!");
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(ownerUser, "Owner");
                await userManager.AddToRoleAsync(ownerUser, "Admin"); // Owner also has Admin privileges
            }
        }

        // Seed Admin User (Existing logic updated)
        var adminUser = await userManager.FindByNameAsync("admin");
        if (adminUser == null)
        {
            adminUser = new ApplicationUser
            {
                UserName = "admin",
                Email = "admin@example.com",
                EmailConfirmed = true,
                FullName = "Yönetici",
                IsAdmin = true,
                CreatedAt = DateTime.Now
            };
            await userManager.CreateAsync(adminUser, "admin123");
            await userManager.AddToRoleAsync(adminUser, "Admin");
        }
        else
        {
            if (!await userManager.IsInRoleAsync(adminUser, "Admin"))
            {
                await userManager.AddToRoleAsync(adminUser, "Admin");
            }
        }
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while migrating the database or seeding Identity.");
    }
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

// Use localization with Turkish as default
app.UseRequestLocalization(app.Services.GetRequiredService<Microsoft.Extensions.Options.IOptions<RequestLocalizationOptions>>().Value);

app.UseHttpsRedirection();

// Generate CSP nonce per request and enforce CSP
app.Use(async (context, next) =>
{
    // Generate base64 nonce
    var nonceBytes = RandomNumberGenerator.GetBytes(16);
    var nonce = Convert.ToBase64String(nonceBytes);
    context.Items["CSPNonce"] = nonce;

    var scriptSrc = app.Environment.IsDevelopment()
        ? $"script-src 'self' https://cdn.jsdelivr.net 'nonce-{nonce}' 'unsafe-eval' 'unsafe-inline'"
        : $"script-src 'self' https://cdn.jsdelivr.net 'nonce-{nonce}'";

    var cspDirectives = new List<string>
    {
        "default-src 'self'",
        scriptSrc,
        // Styles from self and jsdelivr CDN (inline allowed due to existing inline styles)
        "style-src 'self' 'unsafe-inline' https://cdn.jsdelivr.net",
        // Fonts from self and jsdelivr CDN
        "font-src 'self' https://cdn.jsdelivr.net",
        // Images
        "img-src 'self' data:",
        // Allow websocket and HTTP(S) connects to localhost for dev tooling/browser refresh
        "connect-src 'self' https://localhost:* http://localhost:* wss://localhost:* ws://localhost:*",
        // Block object/embed
        "object-src 'none'",
        // Base URI
        "base-uri 'self'"
    };

    context.Response.Headers["Content-Security-Policy"] = string.Join("; ", cspDirectives);

    await next();
});

// Ensure responses declare UTF-8 charset for text/html (fix Turkish characters)
app.Use(async (context, next) =>
{
    context.Response.OnStarting(state =>
    {
        var httpContext = (HttpContext)state!;
        var ct = httpContext.Response.ContentType;
        if (string.IsNullOrEmpty(ct))
        {
            httpContext.Response.ContentType = "text/html; charset=utf-8";
        }
        else if (ct.StartsWith("text/html", StringComparison.OrdinalIgnoreCase) && !ct.Contains("charset", StringComparison.OrdinalIgnoreCase))
        {
            httpContext.Response.ContentType = ct + "; charset=utf-8";
        }
        return Task.CompletedTask;
    }, context);

    await next();
});

// Harden cookies: append Secure and SameSite=Lax for non-auth custom cookies
app.Use(async (context, next) =>
{
    context.Response.OnStarting(() =>
    {
        var setCookieHeaders = context.Response.Headers[HeaderNames.SetCookie];
        if (!string.IsNullOrEmpty(setCookieHeaders))
        {
            var modified = new Microsoft.Extensions.Primitives.StringValues(
                setCookieHeaders.Select(h =>
                {
                    var val = h;
                    if (string.IsNullOrEmpty(val))
                    {
                        return string.Empty;
                    }
                    if (!val.Contains("SameSite", StringComparison.OrdinalIgnoreCase))
                        val += "; SameSite=Lax";
                    if (!val.Contains("Secure", StringComparison.OrdinalIgnoreCase))
                        val += "; Secure";
                    return val;
                }).ToArray());
            context.Response.Headers[HeaderNames.SetCookie] = modified;
        }
        return Task.CompletedTask;
    });

    await next();
});

app.UseStaticFiles();
app.UseCookiePolicy();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.UseSession();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapHub<urunsatisportali.Hubs.GeneralHub>("/general-hub");

app.Run();
