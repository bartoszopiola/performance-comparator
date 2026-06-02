namespace PerformanceComparator.Services
{
    public interface ICsvNavImporter
    {
        /// <summary>
        /// Imports NAV records for a fund from a CSV stream.
        /// Auto-detects Stooq or simple (date,value) format from the header row.
        /// Skips rows whose (FundId, Date) already exists.
        /// </summary>
        Task<ImportResult> ImportAsync(int fundId, Stream csvStream);
    }
}