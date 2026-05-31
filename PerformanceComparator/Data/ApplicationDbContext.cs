using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PerformanceComparator.Models;

namespace PerformanceComparator.Data
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : IdentityDbContext<ApplicationUser>(options)
    {
        // ── Domain tables ────────────────────────────────────────────────────
        public DbSet<AssetClass> AssetClasses { get; set; }
        public DbSet<Fund> Funds { get; set; }
        public DbSet<NavRecord> NavRecords { get; set; }
        public DbSet<ContentBlock> ContentBlocks { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            // Must call base first — configures all Identity tables
            base.OnModelCreating(builder);

            // ── AssetClass ───────────────────────────────────────────────────
            builder.Entity<AssetClass>(e =>
            {
                e.HasIndex(a => a.Slug).IsUnique();
                e.Property(a => a.Name).HasMaxLength(100).IsRequired();
                e.Property(a => a.Slug).HasMaxLength(100).IsRequired();
            });

            // ── Fund ─────────────────────────────────────────────────────────
            builder.Entity<Fund>(e =>
            {
                e.HasIndex(f => f.Ticker).IsUnique();
                e.Property(f => f.Name).HasMaxLength(200).IsRequired();
                e.Property(f => f.Ticker).HasMaxLength(20).IsRequired();
                e.Property(f => f.Currency).HasMaxLength(3).IsRequired();
                e.Property(f => f.LogoPath).HasMaxLength(500);

                e.HasOne(f => f.AssetClass)
                 .WithMany(a => a.Funds)
                 .HasForeignKey(f => f.AssetClassId)
                 .OnDelete(DeleteBehavior.Restrict); // don't cascade-delete funds
            });

            // ── NavRecord ────────────────────────────────────────────────────
            builder.Entity<NavRecord>(e =>
            {
                // One NAV per fund per date — enforced at DB level
                e.HasIndex(n => new { n.FundId, n.Date }).IsUnique();

                e.Property(n => n.Nav)
                 .HasColumnType("TEXT")   // SQLite stores decimals as TEXT for precision
                 .IsRequired();

                e.HasOne(n => n.Fund)
                 .WithMany(f => f.NavRecords)
                 .HasForeignKey(n => n.FundId)
                 .OnDelete(DeleteBehavior.Cascade); // deleting a fund removes its NAV history
            });

            // ── ContentBlock ─────────────────────────────────────────────────
            builder.Entity<ContentBlock>(e =>
            {
                e.HasIndex(c => c.Key).IsUnique();
                e.Property(c => c.Key).HasMaxLength(100).IsRequired();
            });
        }
    }
}