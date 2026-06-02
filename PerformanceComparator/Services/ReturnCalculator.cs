using PerformanceComparator.Models;

namespace PerformanceComparator.Services
{
    /// <summary>
    /// Return metrics.
    ///
    /// Numeric strategy: values are stored as decimal (precision). Pure ratio arithmetic
    /// (daily returns, cumulative) stays in decimal. CAGR needs Math.Pow, which only exists
    /// for double, so that single calculation converts to double for the pow and back to
    /// decimal at the boundary. Documented per CLAUDE.md conventions.
    /// </summary>
    public class ReturnCalculator : IReturnCalculator
    {
        public IReadOnlyList<decimal> DailyReturns(IReadOnlyList<NavRecord> ordered)
        {
            var result = new List<decimal>();
            for (int i = 1; i < ordered.Count; i++)
            {
                var prev = ordered[i - 1].Value;
                var curr = ordered[i].Value;

                // Guard against division by zero — emit 0 for that step.
                if (prev == 0m)
                {
                    result.Add(0m);
                    continue;
                }

                result.Add(curr / prev - 1m);
            }
            return result;
        }

        public decimal CumulativeReturn(IReadOnlyList<NavRecord> ordered)
        {
            if (ordered.Count < 2) return 0m;

            var first = ordered[0].Value;
            var last = ordered[^1].Value;

            if (first == 0m) return 0m;

            return last / first - 1m;
        }

        public decimal Cagr(IReadOnlyList<NavRecord> ordered, int periodsPerYear = 252)
        {
            if (ordered.Count < 2) return 0m;

            int n = ordered.Count - 1; // number of returns
            var first = ordered[0].Value;
            var last = ordered[^1].Value;

            if (first <= 0m || n == 0) return 0m;

            double ratio = (double)(last / first);
            double exponent = (double)periodsPerYear / n;
            double cagr = Math.Pow(ratio, exponent) - 1.0;

            return (decimal)cagr;
        }

        public List<(DateTime Date, decimal Value)> CumulativeReturnSeries(IReadOnlyList<NavRecord> ordered)
        {
            var series = new List<(DateTime, decimal)>();
            if (ordered.Count == 0) return series;

            var baseValue = ordered[0].Value;
            if (baseValue == 0m) return series;

            foreach (var r in ordered)
                series.Add((r.Date, r.Value / baseValue * 100m));

            return series;
        }
    }
}