namespace PerformanceComparator.ViewModels
{
    public class CompareResultViewModel
    {
        public List<FundMetricsViewModel> Funds { get; set; } = [];

        public string BenchmarkName { get; set; } = string.Empty;
        public bool HasBenchmark { get; set; }

        public DateTime Start { get; set; }
        public DateTime End { get; set; }

        // Chart data — one series per fund
        public List<ChartSeriesViewModel> CumulativeSeries { get; set; } = [];
        public List<ChartSeriesViewModel> DrawdownSeries { get; set; } = [];

        // Notes for skipped funds / missing benchmark
        public List<string> Notes { get; set; } = [];
    }
}