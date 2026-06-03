using Microsoft.AspNetCore.Mvc.Rendering;

namespace PerformanceComparator.ViewModels
{
    public class CompareFormViewModel
    {
        public List<SelectListItem> AllFunds { get; set; } = [];
        public List<SelectListItem> BenchmarkFunds { get; set; } = [];

        public DateTime DefaultStart { get; set; }
        public DateTime DefaultEnd { get; set; }
        public decimal DefaultRiskFreeRate { get; set; } = 0.02m;
    }
}