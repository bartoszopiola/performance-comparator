namespace PerformanceComparator.Models
{
    /// <summary>
    /// One NAV (Net Asset Value / Wartość Aktywów Netto) data point for a fund.
    /// Uploaded by the admin via CSV. One row per trading day.
    /// </summary>
    public class NavRecord
    {
        public int Id { get; set; }

        // ── Foreign key ──────────────────────────────────────────────────────
        public int FundId { get; set; }
        public Fund Fund { get; set; } = null!;

        /// <summary>Trading date (no time component needed).</summary>
        public DateOnly Date { get; set; }

        /// <summary>
        /// NAV price in the fund's currency.
        /// Always decimal — never float or double.
        /// </summary>
        public decimal Nav { get; set; }
    }
}