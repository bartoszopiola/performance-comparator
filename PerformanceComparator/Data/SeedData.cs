using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PerformanceComparator.Models;

namespace PerformanceComparator.Data
{
    /// <summary>
    /// Seeds the Admin role, a default admin user, and baseline content blocks.
    /// Default admin: admin@local.test / Admin123! (overridable via AdminSeed config).
    /// </summary>
    public static class SeedData
    {
        public static async Task SeedAsync(IServiceProvider services)
        {
            var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
            var config = services.GetRequiredService<IConfiguration>();
            var context = services.GetRequiredService<ApplicationDbContext>();

            const string adminRole = "Admin";
            if (!await roleManager.RoleExistsAsync(adminRole))
                await roleManager.CreateAsync(new IdentityRole(adminRole));

            var email = config["AdminSeed:Email"] ?? "admin@local.test";
            var password = config["AdminSeed:Password"] ?? "Admin123!";

            var existing = await userManager.FindByEmailAsync(email);
            if (existing is null)
            {
                var admin = new ApplicationUser
                {
                    UserName = email,
                    Email = email,
                    EmailConfirmed = true
                };
                var result = await userManager.CreateAsync(admin, password);
                if (result.Succeeded)
                    await userManager.AddToRoleAsync(admin, adminRole);
            }
            else if (!await userManager.IsInRoleAsync(existing, adminRole))
            {
                await userManager.AddToRoleAsync(existing, adminRole);
            }

            await SeedContentBlockAsync(context, "home.hero",
                "Performance Comparator",
                "Compare Polish investment funds (TFI) and ETFs by return and risk metrics.");

            await SeedContentBlockAsync(context, "home.intro",
                "How it works",
                "Browse funds, analyze their historical performance, and compare them against a chosen benchmark.");

            await SeedContentBlockAsync(context, "about.body",
                "About the project",
                "Performance Comparator is an educational project that presents performance metrics for investment funds. " +
                "All data is for educational purposes only and does not constitute investment advice.");

            await context.SaveChangesAsync();
        }

        private static async Task SeedContentBlockAsync(
            ApplicationDbContext context, string key, string title, string body)
        {
            if (!await context.ContentBlocks.AnyAsync(c => c.Key == key))
            {
                context.ContentBlocks.Add(new ContentBlock
                {
                    Key = key,
                    Title = title,
                    Body = body,
                    UpdatedAt = DateTime.UtcNow
                });
            }
        }
    }
}