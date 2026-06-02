namespace PerformanceComparator.ViewModels.Admin
{
    public class UploadNavViewModel
    {
        public int FundId { get; set; }
        public string FundName { get; set; } = string.Empty;
        public string Symbol { get; set; } = string.Empty;
        public int ExistingNavCount { get; set; }
    }
}