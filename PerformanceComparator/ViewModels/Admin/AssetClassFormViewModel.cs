using System.ComponentModel.DataAnnotations;

namespace PerformanceComparator.ViewModels.Admin
{
    public class AssetClassFormViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Name is required.")]
        [MaxLength(100, ErrorMessage = "Name cannot exceed 100 characters.")]
        public string Name { get; set; } = string.Empty;
    }
}