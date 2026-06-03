using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PerformanceComparator.Models;

namespace PerformanceComparator.Data
{
    /// <summary>
    /// Seeds the database with the Admin role, a default admin user, and baseline
    /// content blocks. Called once from Program.cs before app.Run().
    ///
    /// Default admin credentials (for the university course):
    ///   Email:    admin@local.test
    ///   Password: Admin123!
    /// (Overridable via AdminSeed:Email / AdminSeed:Password in configuration.)
    /// </summary>
    public static class SeedData
    {
        public static async Task SeedAsync(IServiceProvider services)
        {
            var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
            var config = services.GetRequiredService<IConfiguration>();
            var context = services.GetRequiredService<ApplicationDbContext>();

            // ── Admin role ─────────────────────────────────────────────────────
            const string adminRole = "Admin";
            if (!await roleManager.RoleExistsAsync(adminRole))
                await roleManager.CreateAsync(new IdentityRole(adminRole));

            // ── Admin user ─────────────────────────────────────────────────────
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

            // ── Content blocks ─────────────────────────────────────────────────
            await SeedContentBlockAsync(context, "home.hero",
                "Performance Comparator",
                "Porównuj polskie fundusze inwestycyjne (TFI) oraz ETF-y według metryk zwrotu i ryzyka.");

            await SeedContentBlockAsync(context, "home.intro",
                "Jak to działa",
                "Przeglądaj fundusze, analizuj ich historyczne wyniki i porównuj je względem wybranego benchmarku.");

            await SeedContentBlockAsync(context, "about.body",
                "O projekcie",
                "Performance Comparator to projekt edukacyjny prezentujący metryki wynikowe funduszy inwestycyjnych. " +
                "Dane służą wyłącznie celom edukacyjnym i nie stanowią porady inwestycyjnej.");

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