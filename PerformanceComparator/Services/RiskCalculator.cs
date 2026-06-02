using PerformanceComparator.Models;

namespace PerformanceComparator.Services
{
    /// <summary>
    /// Risk metrics.
    ///
    /// Numeric strategy: statistical work (mean, variance, sqrt) is done in double because
    /// .NET has no built-in decimal std dev and Math.Sqrt/Pow are double-only. Inputs are
    /// converted decimal→double at entry and the final result is converted back to decimal
    /// at the public boundary. Storage stays decimal; computation is double. Documented per
    /// CLAUDE.md conventions.
    ///
    /// Division-by-zero policy: any metric whose denominator is zero (e.g. zero volatility,
    /// fewer than 2 observations, no downside) returns 0m rather than throwing.
    /// </summary>
    public class RiskCalculator : IRiskCalculator
    {
        public decimal Volatility(IReadOnlyList<decimal> dailyReturns, int periodsPerYear = 252)
        {
            if (dailyReturns.Count < 2) return 0m;

            double std = SampleStdDev(dailyReturns.Select(d => (double)d).ToList());
            double annualized = std * Math.Sqrt(periodsPerYear);

            return (decimal)annualized;
        }

        public decimal MaxDrawdown(IReadOnlyList<NavRecord> ordered)
        {
            if (ordered.Count == 0) return 0m;

            decimal peak = ordered[0].Value;
            decimal maxDd = 0m;

            foreach (var r in ordered)
            {
                if (r.Value > peak) peak = r.Value;

                if (peak > 0m)
                {
                    decimal dd = r.Value / peak - 1m;
                    if (dd < maxDd) maxDd = dd;
                }
            }

            return maxDd;
        }

        public List<(DateTime Date, decimal Drawdown)> DrawdownSeries(IReadOnlyList<NavRecord> ordered)
        {
            var series = new List<(DateTime, decimal)>();
            if (ordered.Count == 0) return series;

            decimal peak = ordered[0].Value;

            foreach (var r in ordered)
            {
                if (r.Value > peak) peak = r.Value;
                decimal dd = peak > 0m ? r.Value / peak - 1m : 0m;
                series.Add((r.Date, dd));
            }

            return series;
        }

        public decimal Sharpe(IReadOnlyList<decimal> dailyReturns, decimal rfAnnual = 0.02m, int periodsPerYear = 252)
        {
            if (dailyReturns.Count < 2) return 0m;

            double rfPerPeriod = (double)rfAnnual / periodsPerYear;
            var excess = dailyReturns.Select(d => (double)d - rfPerPeriod).ToList();

            double meanExcess = excess.Average();
            double stdExcess = SampleStdDev(excess);

            if (stdExcess == 0.0) return 0m;

            double sharpe = (meanExcess * periodsPerYear) / (stdExcess * Math.Sqrt(periodsPerYear));
            return (decimal)sharpe;
        }

        public decimal Sortino(IReadOnlyList<decimal> dailyReturns, decimal marAnnual = 0m, int periodsPerYear = 252)
        {
            if (dailyReturns.Count < 2) return 0m;

            double marPerPeriod = (double)marAnnual / periodsPerYear;
            var excess = dailyReturns.Select(d => (double)d - marPerPeriod).ToList();

            double meanExcess = excess.Average();

            // Downside deviation: sqrt(mean of squared NEGATIVE excess returns), divided by
            // the TOTAL number of periods (standard Sortino convention).
            double sumSqDownside = excess.Where(e => e < 0).Sum(e => e * e);
            double downsideDev = Math.Sqrt(sumSqDownside / excess.Count);

            if (downsideDev == 0.0) return 0m;

            double sortino = (meanExcess * periodsPerYear) / (downsideDev * Math.Sqrt(periodsPerYear));
            return (decimal)sortino;
        }

        // ── Helper: sample standard deviation (n-1) ──────────────────────────────
        private static double SampleStdDev(IReadOnlyList<double> values)
        {
            int n = values.Count;
            if (n < 2) return 0.0;

            double mean = values.Average();
            double sumSq = 0.0;
            foreach (var v in values)
                sumSq += (v - mean) * (v - mean);

            return Math.Sqrt(sumSq / (n - 1));
        }
    }
}