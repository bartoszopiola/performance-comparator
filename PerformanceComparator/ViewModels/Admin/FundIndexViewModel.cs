namespace PerformanceComparator.ViewModels.Admin
{
    public class FundIndexViewModel
    {
        public List<FundIndexItemViewModel> Funds { get; set; } = [];
    }

    public class FundIndexItemViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Symbol { get; set; } = string.Empty;
        public string AssetClassName { get; set; } = string.Empty;
        public string? Provider { get; set; }
        public string Currency { get; set; } = string.Empty;
        public bool IsBenchmark { get; set; }
        public int NavRecordCount { get; set; }
    }
}