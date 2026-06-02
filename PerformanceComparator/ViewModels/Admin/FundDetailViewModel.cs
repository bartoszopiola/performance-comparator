namespace PerformanceComparator.ViewModels.Admin
{
    public class FundDetailViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Symbol { get; set; } = string.Empty;
        public string AssetClassName { get; set; } = string.Empty;
        public string? Provider { get; set; }
        public string? Description { get; set; }
        public string Currency { get; set; } = string.Empty;
        public bool IsBenchmark { get; set; }
        public DateTime CreatedAt { get; set; }
        public int NavRecordCount { get; set; }
        public DateTime? NavDataFrom { get; set; }
        public DateTime? NavDataTo { get; set; }
        public string? LogoFileName { get; set; }
    }
}