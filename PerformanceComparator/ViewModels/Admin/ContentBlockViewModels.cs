using System.ComponentModel.DataAnnotations;

namespace PerformanceComparator.ViewModels.Admin
{
    public class ContentBlockListItemViewModel
    {
        public int Id { get; set; }
        public string Key { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public DateTime UpdatedAt { get; set; }
    }

    public class ContentBlockFormViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Key is required.")]
        [MaxLength(100)]
        public string Key { get; set; } = string.Empty;

        [MaxLength(200)]
        public string Title { get; set; } = string.Empty;

        [MaxLength(5000)]
        public string Body { get; set; } = string.Empty;

        public DateTime UpdatedAt { get; set; }
    }
}