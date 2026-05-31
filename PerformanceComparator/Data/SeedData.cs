using Microsoft.AspNetCore.Identity;
using PerformanceComparator.Models;

namespace PerformanceComparator.Data
{
    /// <summary>
    /// Seeds the database with the Admin role and a default admin user.
    /// Called once from Program.cs before app.Run().
    ///
    /// Default admin credentials (for the university course):
    ///   Email:    admin@local.test
    ///   Password: Admin123!
    ///
    /// These can be overridden in appsettings.json / appsettings.Development.json:
    ///   "AdminSeed": { "Email": "...", "Password": "..." }
    /// </summary>
    public static class SeedData
    {
        public static async Task SeedAsync(IServiceProvider services)
        {
            var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
            var config = services.GetRequiredService<IConfiguration>();

            // ── Ensure "Admin" role exists ─────────────────────────────────────
            const string adminRole = "Admin";
            if (!await roleManager.RoleExistsAsync(adminRole))
                await roleManager.CreateAsync(new IdentityRole(adminRole));

            // ── Seed admin user ────────────────────────────────────────────────
            // Read credentials from configuration; fall back to documented defaults.
            var email = config["AdminSeed:Email"] ?? "admin@local.test";
            var password = config["AdminSeed:Password"] ?? "Admin123!";

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
                await userManager.AddToRoleAsync(existing, adminRole);
            }
        }
    }
}