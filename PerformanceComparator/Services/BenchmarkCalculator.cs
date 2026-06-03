namespace PerformanceComparator.Services
{
    /// <summary>
    /// Benchmark-relative metrics.
    ///
    /// Numeric strategy (as in the other calculators): align in decimal, compute statistics
    /// in double (variance, covariance, sqrt), convert back to decimal at the boundary.
    /// Division-by-zero policy: returns 0m when there are fewer than 2 aligned observations
    /// or when a denominator (tracking error, benchmark variance) is zero.
    /// </summary>
    public class BenchmarkCalculator : IBenchmarkCalculator
    {
        public decimal TrackingError(
            IReadOnlyList<(DateTime Date, decimal Return)> portfolio,
            IReadOnlyList<(DateTime Date, decimal Return)> benchmark,
            int periodsPerYear = 252)
        {
            var (p, b) = Align(portfolio, benchmark);
            if (p.Count < 2) return 0m;

            var active = new double[p.Count];
            for (int i = 0; i < p.Count; i++)
                active[i] = p[i] - b[i];

            double std = SampleStdDev(active);
            return (decimal)(std * Math.Sqrt(periodsPerYear));
        }

        public decimal InformationRatio(
            IReadOnlyList<(DateTime Date, decimal Return)> portfolio,
            IReadOnlyList<(DateTime Date, decimal Return)> benchmark,
            int periodsPerYear = 252)
        {
            var (p, b) = Align(portfolio, benchmark);
            if (p.Count < 2) return 0m;

            var active = new double[p.Count];
            for (int i = 0; i < p.Count; i++)
                active[i] = p[i] - b[i];

            double meanActive = active.Average();
            double stdActive = SampleStdDev(active);
            if (stdActive == 0.0) return 0m;

            double ir = (meanActive * periodsPerYear) / (stdActive * Math.Sqrt(periodsPerYear));
            return (decimal)ir;
        }

        public decimal Beta(
            IReadOnlyList<(DateTime Date, decimal Return)> portfolio,
            IReadOnlyList<(DateTime Date, decimal Return)> benchmark)
        {
            var (p, b) = Align(portfolio, benchmark);
            if (p.Count < 2) return 0m;

            double varB = SampleVariance(b);
            if (varB == 0.0) return 0m;

            double covar = SampleCovariance(p, b);
            return (decimal)(covar / varB);
        }

        public decimal Alpha(
            IReadOnlyList<(DateTime Date, decimal Return)> portfolio,
            IReadOnlyList<(DateTime Date, decimal Return)> benchmark,
            decimal rfAnnual = 0.02m,
            int periodsPerYear = 252)
        {
            var (p, b) = Align(portfolio, benchmark);
            if (p.Count < 2) return 0m;

            double varB = SampleVariance(b);
            if (varB == 0.0) return 0m;

            double beta = SampleCovariance(p, b) / varB;
            double rfPeriod = (double)rfAnnual / periodsPerYear;
            double meanP = p.Average();
            double meanB = b.Average();

            // Jensen's alpha per period, then annualized
            double alphaPeriod = meanP - (rfPeriod + beta * (meanB - rfPeriod));
            double alphaAnnual = alphaPeriod * periodsPerYear;

            return (decimal)alphaAnnual;
        }

        // ── Alignment: inner join on Date ────────────────────────────────────────
        private static (List<double> p, List<double> b) Align(
            IReadOnlyList<(DateTime Date, decimal Return)> portfolio,
            IReadOnlyList<(DateTime Date, decimal Return)> benchmark)
        {
            var benchMap = new Dictionary<DateTime, decimal>();
            foreach (var pt in benchmark)
                benchMap[pt.Date] = pt.Return;

            var p = new List<double>();
            var b = new List<double>();

            foreach (var pt in portfolio)
            {
                if (benchMap.TryGetValue(pt.Date, out var br))
                {
                    p.Add((double)pt.Return);
                    b.Add((double)br);
                }
            }

            return (p, b);
        }

        // ── Statistics helpers (sample, n-1) ─────────────────────────────────────
        private static double SampleStdDev(IReadOnlyList<double> values)
            => Math.Sqrt(SampleVariance(values));

        private static double SampleVariance(IReadOnlyList<double> values)
        {
            int n = values.Count;
            if (n < 2) return 0.0;

            double mean = values.Average();
            double sumSq = 0.0;
            foreach (var v in values)
                sumSq += (v - mean) * (v - mean);

            return sumSq / (n - 1);
        }

        private static double SampleCovariance(IReadOnlyList<double> x, IReadOnlyList<double> y)
        {
            int n = x.Count;
            if (n < 2) return 0.0;

            double meanX = x.Average();
            double meanY = y.Average();
            double sum = 0.0;
            for (int i = 0; i < n; i++)
                sum += (x[i] - meanX) * (y[i] - meanY);

            return sum / (n - 1);
        }
    }
}