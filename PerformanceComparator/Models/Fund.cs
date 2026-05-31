namespace PerformanceComparator.Models
{
    public class Fund
    {
        public int Id { get; set; }

        /// <summary>Full fund name, e.g. "PKO Akcji Plus", "Vanguard FTSE All-World UCITS ETF"</summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>Short ticker/code, e.g. "PKO015", "VWCE"</summary>
        public string Ticker { get; set; } = string.Empty;

        public string? Description { get; set; }

        /// <summary>Relative path under wwwroot/, e.g. "uploads/logos/abc123.png"</summary>
        public string? LogoPath { get; set; }

        /// <summary>ISO 4217 currency code, e.g. "PLN", "USD", "EUR"</summary>
        public string Currency { get; set; } = "PLN";

        public bool IsActive { get; set; } = true;

        // ── Foreign key ──────────────────────────────────────────────────────
        public int AssetClassId { get; set; }
        public AssetClass AssetClass { get; set; } = null!;

        public ICollection<NavRecord> NavRecords { get; set; } = [];
    }
}