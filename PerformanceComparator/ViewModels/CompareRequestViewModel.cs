using System.ComponentModel.DataAnnotations;

namespace PerformanceComparator.ViewModels
{
    public class CompareRequestViewModel
    {
        /// <summary>IDs of funds to compare (1–4). Bound from multiple hidden inputs named "FundIds".</summary>
        public int[] FundIds { get; set; } = [];

        [Required(ErrorMessage = "Select a benchmark.")]
        [Range(1, int.MaxValue, ErrorMessage = "Select a benchmark.")]
        public int BenchmarkId { get; set; }

        [Required]
        public DateTime Start { get; set; }

        [Required]
        public DateTime End { get; set; }

        /// <summary>User enters as a percentage (e.g. 2 = 2%). Controller divides by 100 before passing to services.</summary>
        [Range(0, 100, ErrorMessage = "Risk-free rate must be between 0 and 100.")]
        public decimal RiskFreeRate { get; set; } = 2m;
    }
}