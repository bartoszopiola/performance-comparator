using System.ComponentModel.DataAnnotations;

namespace PerformanceComparator.Models
{
    public class AssetClass
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        public ICollection<Fund> Funds { get; set; } = [];
    }
}