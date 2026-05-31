using Microsoft.AspNetCore.Identity;
using PerformanceComparator.Models;

namespace PerformanceComparator.Data
{
    /// <summary>
    /// Seeds the database with required baseline data on first run.
    /// Called once from Program.cs before app.Run().
    /// </summary>
    public static class DbInitializer
    {
        public static async Task SeedAsync(IServiceProvider services)
        {
            var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
            var config = services.GetRequiredService<IConfiguration>();

            // ── Seed Admin role ───────────────────────────────────────────────
            const string adminRole = "Admin";
            if (!await roleManager.RoleExistsAsync(adminRole))
                await roleManager.CreateAsync(new IdentityRole(adminRole));

            // ── Seed Admin user ───────────────────────────────────────────────
            // Credentials come from appsettings.Development.json so they are
            // never committed to source control.
            // Add this block to appsettings.Development.json:
            //
            // "AdminSeed": {
            //   "Email": "admin@localhost",
            //   "Password": "Admin1234!"
            // }
            //
            var email = config["AdminSeed:Email"];
            var password = config["AdminSeed:Password"];

            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                // No seed config — skip silently in production; log in development
                var logger = services.GetRequiredService<ILogger<Program>>();
                logger.LogWarning(
                    "AdminSeed config missing. Add AdminSeed:Email and AdminSeed:Password " +
                    "to appsettings.Development.json to create the default admin account.");
                return;
            }

            var existing = await userManager.FindByEmailAsync(email);
            if (existing is null)
            {
                var admin = new ApplicationUser
                {
                    UserName = email,
                    Email = email,
                    EmailConfirmed = true   // skip email confirmation for seed account
                };

                var result = await userManager.CreateAsync(admin, password);
                if (result.Succeeded)
                    await userManager.AddToRoleAsync(admin, adminRole);
            }
            else if (!await userManager.IsInRoleAsync(existing, adminRole))
            {
                // User exists but lost the role somehow — restore it
                await userManager.AddToRoleAsync(existing, adminRole);
            }
        }
    }
}