namespace PerformanceComparator.ViewModels
{
    public class FundsListViewModel
    {
        public List<FundCardViewModel> Funds { get; set; } = [];

        /// <summary>Distinct asset class names for the client-side filter dropdown.</summary>
        public List<string> AssetClasses { get; set; } = [];
    }

    public class FundCardViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Symbol { get; set; } = string.Empty;
        public string AssetClassName { get; set; } = string.Empty;
        public string? Provider { get; set; }
        public string? LogoFileName { get; set; }
        public bool IsBenchmark { get; set; }
        public int NavCount { get; set; }
        public DateTime? NavDataFrom { get; set; }
        public DateTime? NavDataTo { get; set; }
    }
}