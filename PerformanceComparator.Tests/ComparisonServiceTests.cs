using Microsoft.EntityFrameworkCore;
using PerformanceComparator.Data;
using PerformanceComparator.Models;
using PerformanceComparator.Services;
using Xunit;

namespace PerformanceComparator.Tests
{
    public class ComparisonServiceTests
    {
        private static ApplicationDbContext NewContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            return new ApplicationDbContext(options);
        }

        private static ComparisonService NewService(ApplicationDbContext context)
        {
            return new ComparisonService(
                context,
                new ReturnCalculator(),
                new RiskCalculator(),
                new BenchmarkCalculator());
        }

        private static (int fundA, int fundB, int benchmark) Seed(ApplicationDbContext context)
        {
            var ac = new AssetClass { Name = "Test Class" };
            context.AssetClasses.Add(ac);
            context.SaveChanges();

            var fundA = new Fund { Name = "Fund A", Symbol = "AAA", AssetClassId = ac.Id, Currency = "PLN", CreatedAt = DateTime.UtcNow };
            var fundB = new Fund { Name = "Fund B", Symbol = "BBB", AssetClassId = ac.Id, Currency = "PLN", CreatedAt = DateTime.UtcNow };
            var bench = new Fund { Name = "Benchmark", Symbol = "BMK", AssetClassId = ac.Id, Currency = "PLN", IsBenchmark = true, CreatedAt = DateTime.UtcNow };
            context.Funds.AddRange(fundA, fundB, bench);
            context.SaveChanges();

            var start = new DateTime(2024, 1, 1);

            decimal[] aVals = [100, 101, 103, 102, 104, 106, 105, 108, 110, 109];
            decimal[] bVals = [100, 100, 101, 103, 102, 104, 107, 106, 108, 111];
            decimal[] kVals = [100, 101, 102, 102, 103, 105, 106, 107, 108, 110];

            for (int i = 0; i < 10; i++)
            {
                var date = start.AddDays(i);
                context.NavRecords.Add(new NavRecord { FundId = fundA.Id, Date = date, Value = aVals[i] });
                context.NavRecords.Add(new NavRecord { FundId = fundB.Id, Date = date, Value = bVals[i] });
                context.NavRecords.Add(new NavRecord { FundId = bench.Id, Date = date, Value = kVals[i] });
            }
            context.SaveChanges();

            return (fundA.Id, fundB.Id, bench.Id);
        }

        [Fact]
        public async Task CompareAsync_TwoFunds_ReturnsBothWithMetrics()
        {
            using var context = NewContext();
            var (fundA, fundB, benchmark) = Seed(context);
            var service = NewService(context);

            var result = await service.CompareAsync(
                fundIds: [fundA, fundB],
                benchmarkId: benchmark,
                start: new DateTime(2024, 1, 1),
                end: new DateTime(2024, 1, 31),
                rfAnnual: 0.02m);

            Assert.Equal(2, result.Funds.Count);
            Assert.True(result.HasBenchmark);
            Assert.Equal("Benchmark", result.BenchmarkName);
            Assert.All(result.Funds, f => Assert.NotEqual(0m, f.CumulativeReturn));
            Assert.Equal(2, result.CumulativeSeries.Count);
            Assert.Equal(2, result.DrawdownSeries.Count);
            Assert.All(result.CumulativeSeries, s => Assert.Equal(100m, s.Points[0].Value, 4));
        }

        [Fact]
        public async Task CompareAsync_MissingBenchmark_AddsNoteAndStillReturnsFunds()
        {
            using var context = NewContext();
            var (fundA, _, _) = Seed(context);
            var service = NewService(context);

            var result = await service.CompareAsync(
                fundIds: [fundA],
                benchmarkId: 9999,
                start: new DateTime(2024, 1, 1),
                end: new DateTime(2024, 1, 31),
                rfAnnual: 0.02m);

            Assert.Single(result.Funds);
            Assert.False(result.HasBenchmark);
            Assert.NotEmpty(result.Notes);
        }

        [Fact]
        public async Task CompareAsync_FundWithNoDataInRange_IsSkippedWithNote()
        {
            using var context = NewContext();
            var (fundA, _, benchmark) = Seed(context);
            var service = NewService(context);

            var result = await service.CompareAsync(
                fundIds: [fundA],
                benchmarkId: benchmark,
                start: new DateTime(2030, 1, 1),
                end: new DateTime(2030, 12, 31),
                rfAnnual: 0.02m);

            Assert.Empty(result.Funds);
            Assert.NotEmpty(result.Notes);
        }
    }
}