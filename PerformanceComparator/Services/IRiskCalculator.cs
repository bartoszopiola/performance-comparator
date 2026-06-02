using PerformanceComparator.Models;

namespace PerformanceComparator.Services
{
    public interface IRiskCalculator
    {
        /// <summary>Annualized volatility: sample std dev (n-1) of daily returns * sqrt(periodsPerYear).</summary>
        decimal Volatility(IReadOnlyList<decimal> dailyReturns, int periodsPerYear = 252);

        /// <summary>Maximum drawdown as a negative decimal (e.g. -0.20 = -20%).</summary>
        decimal MaxDrawdown(IReadOnlyList<NavRecord> ordered);

        /// <summary>Drawdown at each point (negative or zero), for an underwater chart.</summary>
        List<(DateTime Date, decimal Drawdown)> DrawdownSeries(IReadOnlyList<NavRecord> ordered);

        /// <summary>Annualized Sharpe ratio. rfAnnual is the annual risk-free rate (decimal).</summary>
        decimal Sharpe(IReadOnlyList<decimal> dailyReturns, decimal rfAnnual = 0.02m, int periodsPerYear = 252);

        /// <summary>Annualized Sortino ratio. marAnnual is the annual minimum acceptable return.</summary>
        decimal Sortino(IReadOnlyList<decimal> dailyReturns, decimal marAnnual = 0m, int periodsPerYear = 252);
    }
}