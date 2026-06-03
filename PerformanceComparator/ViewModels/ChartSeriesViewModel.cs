namespace PerformanceComparator.ViewModels
{
    public class ChartSeriesViewModel
    {
        public string Label { get; set; } = string.Empty;
        public List<(DateTime Date, decimal Value)> Points { get; set; } = [];
    }
}