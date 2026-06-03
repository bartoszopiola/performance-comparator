namespace PerformanceComparator.ViewModels
{
    public class FundMetricsViewModel
    {
        public int FundId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Symbol { get; set; } = string.Empty;

        // ── Absolute metrics ──────────────────────────────────────────────────
        public decimal CumulativeReturn { get; set; }
        public decimal Cagr { get; set; }
        public decimal Volatility { get; set; }
        public decimal MaxDrawdown { get; set; }
        public decimal Sharpe { get; set; }
        public decimal Sortino { get; set; }

        // ── Benchmark-relative metrics ────────────────────────────────────────
        public decimal Beta { get; set; }
        public decimal Alpha { get; set; }
        public decimal TrackingError { get; set; }
        public decimal InformationRatio { get; set; }

        public bool IsBenchmark { get; set; }
    }
}