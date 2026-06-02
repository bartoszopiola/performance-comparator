using PerformanceComparator.Models;

namespace PerformanceComparator.Services
{
    public interface IReturnCalculator
    {
        /// <summary>Daily returns P_t / P_{t-1} - 1 for consecutive ordered records.</summary>
        IReadOnlyList<decimal> DailyReturns(IReadOnlyList<NavRecord> ordered);

        /// <summary>Total return over the period: last/first - 1.</summary>
        decimal CumulativeReturn(IReadOnlyList<NavRecord> ordered);

        /// <summary>Compound annual growth rate: (1+cumulative)^(periodsPerYear/n) - 1, n = number of returns.</summary>
        decimal Cagr(IReadOnlyList<NavRecord> ordered, int periodsPerYear = 252);

        /// <summary>Series normalized to 100 at the start, for charting.</summary>
        List<(DateTime Date, decimal Value)> CumulativeReturnSeries(IReadOnlyList<NavRecord> ordered);
    }
}