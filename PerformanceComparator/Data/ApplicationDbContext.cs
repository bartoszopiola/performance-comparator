using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PerformanceComparator.Models;

namespace PerformanceComparator.Data
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : IdentityDbContext<ApplicationUser>(options)
    {
        public DbSet<AssetClass> AssetClasses { get; set; }
        public DbSet<Fund> Funds { get; set; }
        public DbSet<NavRecord> NavRecords { get; set; }
        public DbSet<ContentBlock> ContentBlocks { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // ── Fund → AssetClass (many-to-one) ───────────────────────────────
            builder.Entity<Fund>()
                .HasOne(f => f.AssetClass)
                .WithMany(a => a.Funds)
                .HasForeignKey(f => f.AssetClassId)
                .OnDelete(DeleteBehavior.Restrict);

            // ── NavRecord → Fund (many-to-one) ────────────────────────────────
            builder.Entity<NavRecord>()
                .HasOne(n => n.Fund)
                .WithMany(f => f.NavRecords)
                .HasForeignKey(n => n.FundId)
                .OnDelete(DeleteBehavior.Cascade);

            // ── NavRecord.Value precision ─────────────────────────────────────
            builder.Entity<NavRecord>()
                .Property(n => n.Value)
                .HasPrecision(18, 6);

            // ── Unique index: no duplicate (FundId, Date) ─────────────────────
            builder.Entity<NavRecord>()
                .HasIndex(n => new { n.FundId, n.Date })
                .IsUnique();

            // ── Unique index: ContentBlock.Key ────────────────────────────────
            builder.Entity<ContentBlock>()
                .HasIndex(c => c.Key)
                .IsUnique();
        }
    }
}