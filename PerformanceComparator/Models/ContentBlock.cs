using System.ComponentModel.DataAnnotations;

namespace PerformanceComparator.Models
{
    public class ContentBlock
    {
        public int Id { get; set; }

        /// <summary>e.g. "home.hero", "about.body"</summary>
        [Required]
        [MaxLength(100)]
        public string Key { get; set; } = string.Empty;

        [MaxLength(200)]
        public string Title { get; set; } = string.Empty;

        /// <summary>Can hold HTML or markdown</summary>
        [MaxLength(5000)]
        public string Body { get; set; } = string.Empty;

        public DateTime UpdatedAt { get; set; }
    }
}