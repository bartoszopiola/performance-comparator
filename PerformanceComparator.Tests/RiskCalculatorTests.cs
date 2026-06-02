using PerformanceComparator.Models;
using PerformanceComparator.Services;
using Xunit;

namespace PerformanceComparator.Tests
{
    public class RiskCalculatorTests
    {
        private readonly RiskCalculator _calc = new();

        private static List<NavRecord> Records(params decimal[] values)
        {
            var start = new DateTime(2024, 1, 1);
            var list = new List<NavRecord>();
            for (int i = 0; i < values.Length; i++)
                list.Add(new NavRecord { Date = start.AddDays(i), Value = values[i] });
            return list;
        }

        // Manual: values [100,110,105,90,95,120]
        //   running peak: 100,110,110,110,110,120
        //   lowest point vs its peak: 90 vs 110 -> 90/110 - 1 = -0.181818...
        [Fact]
        public void MaxDrawdown_KnownSeries_ReturnsMinusEighteenPercent()
        {
            var result = _calc.MaxDrawdown(Records(100m, 110m, 105m, 90m, 95m, 120m));

            Assert.Equal(-0.1818m, result, 4);
        }

        // Manual: returns [0.01, 0.02, -0.01, 0.03]
        //   mean = 0.0125
        //   squared devs sum = 0.000875 ; sample var = 0.000875/3 = 0.000291667
        //   sample std = sqrt(0.000291667) = 0.0170782...
        //   with periodsPerYear = 1, annualization factor sqrt(1) = 1
        [Fact]
        public void Volatility_KnownReturns_DailyEqualsSampleStdDev()
        {
            var returns = new List<decimal> { 0.01m, 0.02m, -0.01m, 0.03m };

            var result = _calc.Volatility(returns, periodsPerYear: 1);

            Assert.Equal(0.0171m, result, 4);
        }

        // Manual: same returns, rf = 0, periodsPerYear = 1
        //   mean = 0.0125 ; sample std = 0.0170782
        //   Sharpe = (0.0125 * 1) / (0.0170782 * 1) = 0.731925
        [Fact]
        public void Sharpe_KnownReturns_RfZero_ReturnsExpected()
        {
            var returns = new List<decimal> { 0.01m, 0.02m, -0.01m, 0.03m };

            var result = _calc.Sharpe(returns, rfAnnual: 0m, periodsPerYear: 1);

            Assert.Equal(0.7319m, result, 4);
        }

        [Fact]
        public void Volatility_TooFewReturns_ReturnsZero()
        {
            Assert.Equal(0m, _calc.Volatility(new List<decimal> { 0.01m }));
        }

        [Fact]
        public void MaxDrawdown_AlwaysRising_ReturnsZero()
        {
            var result = _calc.MaxDrawdown(Records(100m, 110m, 120m, 130m));

            Assert.Equal(0m, result, 4);
        }

        [Fact]
        public void DrawdownSeries_HasEntryPerRecord()
        {
            var series = _calc.DrawdownSeries(Records(100m, 110m, 90m));

            Assert.Equal(3, series.Count);
            Assert.Equal(0m, series[0].Drawdown, 4);          // at peak
            Assert.Equal(0m, series[1].Drawdown, 4);          // new peak
            Assert.Equal(90m / 110m - 1m, series[2].Drawdown, 4); // underwater
        }
    }
}