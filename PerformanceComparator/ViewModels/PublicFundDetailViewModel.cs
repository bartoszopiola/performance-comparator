namespace PerformanceComparator.ViewModels
{
    public class PublicFundDetailViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Symbol { get; set; } = string.Empty;
        public string AssetClassName { get; set; } = string.Empty;
        public string? Provider { get; set; }
        public string? Description { get; set; }
        public string? LogoFileName { get; set; }
        public string Currency { get; set; } = string.Empty;

        public int NavCount { get; set; }
        public DateTime? NavDataFrom { get; set; }
        public DateTime? NavDataTo { get; set; }

        public bool HasEnoughData { get; set; }

        // ── Metrics over all available data ───────────────────────────────────
        public decimal CumulativeReturn { get; set; }
        public decimal Cagr { get; set; }
        public decimal Volatility { get; set; }
        public decimal MaxDrawdown { get; set; }
        public decimal Sharpe { get; set; }
        public decimal Sortino { get; set; }

        // ── Chart data ────────────────────────────────────────────────────────
        public List<(DateTime Date, decimal Value)> CumulativePoints { get; set; } = [];
    }
}