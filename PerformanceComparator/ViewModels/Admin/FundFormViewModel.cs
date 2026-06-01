using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace PerformanceComparator.ViewModels.Admin
{
    public class FundFormViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Name is required.")]
        [MaxLength(200, ErrorMessage = "Name cannot exceed 200 characters.")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Symbol is required.")]
        [MaxLength(50, ErrorMessage = "Symbol cannot exceed 50 characters.")]
        public string Symbol { get; set; } = string.Empty;

        [Display(Name = "Asset Class")]
        [Required(ErrorMessage = "Asset Class is required.")]
        public int AssetClassId { get; set; }

        [MaxLength(200)]
        public string? Provider { get; set; }

        [MaxLength(2000)]
        public string? Description { get; set; }

        [MaxLength(3)]
        public string Currency { get; set; } = "PLN";

        [Display(Name = "Is Benchmark")]
        public bool IsBenchmark { get; set; }

        // ── Dropdown data (not bound on POST) ────────────────────────────────
        public SelectList? AssetClasses { get; set; }
    }
}