namespace PerformanceComparator.Services
{
    /// <summary>
    /// Benchmark-relative metrics. All methods first align the two return series by date
    /// (inner join — only dates present in BOTH are used), so a Polish fund and a US ETF
    /// with different trading calendars are compared only on their overlapping dates.
    /// Returns are passed as date-tagged daily returns.
    /// </summary>
    public interface IBenchmarkCalculator
    {
        decimal TrackingError(
            IReadOnlyList<(DateTime Date, decimal Return)> portfolio,
            IReadOnlyList<(DateTime Date, decimal Return)> benchmark,
            int periodsPerYear = 252);

        decimal InformationRatio(
            IReadOnlyList<(DateTime Date, decimal Return)> portfolio,
            IReadOnlyList<(DateTime Date, decimal Return)> benchmark,
            int periodsPerYear = 252);

        decimal Beta(
            IReadOnlyList<(DateTime Date, decimal Return)> portfolio,
            IReadOnlyList<(DateTime Date, decimal Return)> benchmark);

        decimal Alpha(
            IReadOnlyList<(DateTime Date, decimal Return)> portfolio,
            IReadOnlyList<(DateTime Date, decimal Return)> benchmark,
            decimal rfAnnual = 0.02m,
            int periodsPerYear = 252);
    }
}