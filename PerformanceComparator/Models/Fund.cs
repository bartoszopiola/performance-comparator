using System.ComponentModel.DataAnnotations;

namespace PerformanceComparator.Models
{
    public class Fund
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(200)]
        public string Name { get; set; } = string.Empty;

        /// <summary>e.g. "SPY" or a Stooq symbol</summary>
        [Required]
        [MaxLength(50)]
        public string Symbol { get; set; } = string.Empty;

        // ── Foreign key ──────────────────────────────────────────────────────
        public int AssetClassId { get; set; }
        public AssetClass AssetClass { get; set; } = null!;

        /// <summary>e.g. "PKO TFI", "Vanguard"</summary>
        [MaxLength(200)]
        public string? Provider { get; set; }

        [MaxLength(2000)]
        public string? Description { get; set; }

        /// <summary>Filename in wwwroot/uploads/logos</summary>
        public string? LogoFileName { get; set; }

        [MaxLength(3)]
        public string Currency { get; set; } = "PLN";

        public bool IsBenchmark { get; set; } = false;

        public DateTime CreatedAt { get; set; }

        public ICollection<NavRecord> NavRecords { get; set; } = [];
    }
}