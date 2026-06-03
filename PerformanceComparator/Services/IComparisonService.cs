using PerformanceComparator.ViewModels;

namespace PerformanceComparator.Services
{
    public interface IComparisonService
    {
        /// <summary>
        /// Loads NAV data for the requested funds and benchmark within [start, end],
        /// computes absolute and benchmark-relative metrics, and builds chart series.
        /// Funds with no data in range are skipped with a note.
        /// </summary>
        Task<CompareResultViewModel> CompareAsync(
            int[] fundIds,
            int benchmarkId,
            DateTime start,
            DateTime end,
            decimal rfAnnual);
    }
}