namespace PerformanceComparator.Models
{
    public class AssetClass
    {
        public int Id { get; set; }

        /// <summary>Display name, e.g. "Akcji polskich", "Obligacji", "ETF Globalny"</summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>URL-safe identifier, e.g. "akcji-polskich". Used in routes.</summary>
        public string Slug { get; set; } = string.Empty;

        public ICollection<Fund> Funds { get; set; } = [];
    }
}