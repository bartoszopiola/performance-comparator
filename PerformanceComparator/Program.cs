using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PerformanceComparator.Data;
using PerformanceComparator.Models;
using PerformanceComparator.Services;

var builder = WebApplication.CreateBuilder(args);

// ── Database ─────────────────────────────────────────────────────────────────
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(connectionString));

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

// ── Identity ──────────────────────────────────────────────────────────────────
builder.Services.AddDefaultIdentity<ApplicationUser>(options =>
{
    options.SignIn.RequireConfirmedAccount = false; // simplified for university project
})
    .AddRoles<IdentityRole>()                          // enables [Authorize(Roles = "Admin")]
    .AddEntityFrameworkStores<ApplicationDbContext>();

// ── MVC ───────────────────────────────────────────────────────────────────────
builder.Services.AddControllersWithViews();

// ── Application services ───────────────────────────────────────────────────────
builder.Services.AddScoped<ICsvNavImporter, CsvNavImporter>();
// builder.Services.AddScoped<IPerformanceService, PerformanceService>();

// ── File upload size limit (10 MB) ────────────────────────────────────────────
builder.Services.Configure<Microsoft.AspNetCore.Http.Features.FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = 10 * 1024 * 1024; // 10 MB
});

var app = builder.Build();

// ── Seed database (Admin role + user) ─────────────────────────────────────────
using (var scope = app.Services.CreateScope())
{
    await SeedData.SeedAsync(scope.ServiceProvider);
}

// ── HTTP pipeline ─────────────────────────────────────────────────────────────
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthentication();   // must come before UseAuthorization
app.UseAuthorization();

app.MapStaticAssets();

// ── Routes ────────────────────────────────────────────────────────────────────
app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.MapRazorPages()
    .WithStaticAssets();

app.Run();