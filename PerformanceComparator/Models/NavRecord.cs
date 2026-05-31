namespace PerformanceComparator.Models
{
    public class NavRecord
    {
        public int Id { get; set; }

        // ── Foreign key ──────────────────────────────────────────────────────
        public int FundId { get; set; }
        public Fund Fund { get; set; } = null!;

        public DateTime Date { get; set; }

        public decimal Value { get; set; }
    }
}