using PerformanceComparator.Models;
using PerformanceComparator.Services;
using Xunit;

namespace PerformanceComparator.Tests
{
    public class ReturnCalculatorTests
    {
        private readonly ReturnCalculator _calc = new();

        // Helper: build ordered NavRecords from values, one day apart starting 2024-01-01.
        private static List<NavRecord> Records(params decimal[] values)
        {
            var start = new DateTime(2024, 1, 1);
            var list = new List<NavRecord>();
            for (int i = 0; i < values.Length; i++)
                list.Add(new NavRecord { Date = start.AddDays(i), Value = values[i] });
            return list;
        }

        // Manual: [100, 110, 105]
        //   110/100 - 1 = 0.10
        //   105/110 - 1 = -0.0454545...
        [Fact]
        public void DailyReturns_KnownValues_ReturnsExpected()
        {
            var result = _calc.DailyReturns(Records(100m, 110m, 105m));

            Assert.Equal(2, result.Count);
            Assert.Equal(0.10m, result[0], 4);
            Assert.Equal(-0.0455m, result[1], 4);
        }

        // Manual: last/first - 1 = 105/100 - 1 = 0.05
        [Fact]
        public void CumulativeReturn_KnownValues_ReturnsFivePercent()
        {
            var result = _calc.CumulativeReturn(Records(100m, 110m, 105m));

            Assert.Equal(0.05m, result, 4);
        }

        // Manual sanity: a series that exactly doubles over 252 returns (253 records).
        //   CAGR = (200/100)^(252/252) - 1 = 2^1 - 1 = 1.0  (100%)
        [Fact]
        public void Cagr_DoublingOverOneYear_ReturnsOneHundredPercent()
        {
            var start = new DateTime(2024, 1, 1);
            var records = new List<NavRecord>();
            for (int i = 0; i <= 252; i++)
            {
                decimal value = i == 0 ? 100m : (i == 252 ? 200m : 150m);
                records.Add(new NavRecord { Date = start.AddDays(i), Value = value });
            }

            var result = _calc.Cagr(records, 252);

            Assert.Equal(1.0m, result, 4);
        }

        [Fact]
        public void CumulativeReturnSeries_NormalizedToOneHundredAtStart()
        {
            var series = _calc.CumulativeReturnSeries(Records(100m, 110m, 105m));

            Assert.Equal(3, series.Count);
            Assert.Equal(100m, series[0].Value, 4);   // base
            Assert.Equal(110m, series[1].Value, 4);   // 110/100*100
            Assert.Equal(105m, series[2].Value, 4);   // 105/100*100
        }

        [Fact]
        public void CumulativeReturn_TooFewRecords_ReturnsZero()
        {
            Assert.Equal(0m, _calc.CumulativeReturn(Records(100m)));
        }
    }
}