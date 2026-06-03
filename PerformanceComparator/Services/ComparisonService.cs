using Microsoft.EntityFrameworkCore;
using PerformanceComparator.Data;
using PerformanceComparator.Models;
using PerformanceComparator.ViewModels;

namespace PerformanceComparator.Services
{
    public class ComparisonService : IComparisonService
    {
        private readonly ApplicationDbContext _context;
        private readonly IReturnCalculator _returns;
        private readonly IRiskCalculator _risk;
        private readonly IBenchmarkCalculator _benchmark;

        public ComparisonService(
            ApplicationDbContext context,
            IReturnCalculator returns,
            IRiskCalculator risk,
            IBenchmarkCalculator benchmark)
        {
            _context = context;
            _returns = returns;
            _risk = risk;
            _benchmark = benchmark;
        }

        public async Task<CompareResultViewModel> CompareAsync(
            int[] fundIds,
            int benchmarkId,
            DateTime start,
            DateTime end,
            decimal rfAnnual)
        {
            var result = new CompareResultViewModel
            {
                Start = start,
                End = end
            };

            // ── Load benchmark (if any) ────────────────────────────────────────
            Fund? benchmarkFund = await _context.Funds
                .FirstOrDefaultAsync(f => f.Id == benchmarkId);

            List<NavRecord> benchmarkRecords = [];
            List<(DateTime Date, decimal Return)> benchmarkReturns = [];

            if (benchmarkFund is null)
            {
                result.Notes.Add("Benchmark not found — benchmark-relative metrics were not computed.");
            }
            else
            {
                benchmarkRecords = await LoadRecordsAsync(benchmarkId, start, end);
                if (benchmarkRecords.Count < 2)
                {
                    result.Notes.Add($"Benchmark '{benchmarkFund.Name}' has no data in range — benchmark-relative metrics were not computed.");
                    benchmarkFund = null;
                }
                else
                {
                    result.HasBenchmark = true;
                    result.BenchmarkName = benchmarkFund.Name;
                    benchmarkReturns = DatedReturns(benchmarkRecords);
                }
            }

            // ── Process each fund ──────────────────────────────────────────────
            foreach (var fundId in fundIds)
            {
                var fund = await _context.Funds.FirstOrDefaultAsync(f => f.Id == fundId);
                if (fund is null)
                {
                    result.Notes.Add($"Fund id {fundId} not found — skipped.");
                    continue;
                }

                var records = await LoadRecordsAsync(fundId, start, end);
                if (records.Count < 2)
                {
                    result.Notes.Add($"Fund '{fund.Name}' has no usable data in range — skipped.");
                    continue;
                }

                var dailyReturns = _returns.DailyReturns(records);
                var datedReturns = DatedReturns(records);

                var metrics = new FundMetricsViewModel
                {
                    FundId = fund.Id,
                    Name = fund.Name,
                    Symbol = fund.Symbol,
                    IsBenchmark = benchmarkFund is not null && fund.Id == benchmarkFund.Id,

                    CumulativeReturn = _returns.CumulativeReturn(records),
                    Cagr = _returns.Cagr(records),
                    Volatility = _risk.Volatility(dailyReturns),
                    MaxDrawdown = _risk.MaxDrawdown(records),
                    Sharpe = _risk.Sharpe(dailyReturns, rfAnnual),
                    Sortino = _risk.Sortino(dailyReturns)
                };

                // Benchmark-relative metrics (only if a valid benchmark exists)
                if (benchmarkFund is not null)
                {
                    metrics.Beta = _benchmark.Beta(datedReturns, benchmarkReturns);
                    metrics.Alpha = _benchmark.Alpha(datedReturns, benchmarkReturns, rfAnnual);
                    metrics.TrackingError = _benchmark.TrackingError(datedReturns, benchmarkReturns);
                    metrics.InformationRatio = _benchmark.InformationRatio(datedReturns, benchmarkReturns);
                }

                result.Funds.Add(metrics);

                // Chart series
                result.CumulativeSeries.Add(new ChartSeriesViewModel
                {
                    Label = fund.Name,
                    Points = _returns.CumulativeReturnSeries(records)
                });

                var drawdown = _risk.DrawdownSeries(records);
                result.DrawdownSeries.Add(new ChartSeriesViewModel
                {
                    Label = fund.Name,
                    Points = drawdown.Select(d => (d.Date, d.Drawdown)).ToList()
                });
            }

            return result;
        }

        // ── Helpers ──────────────────────────────────────────────────────────────
        private async Task<List<NavRecord>> LoadRecordsAsync(int fundId, DateTime start, DateTime end)
        {
            return await _context.NavRecords
                .Where(n => n.FundId == fundId && n.Date >= start && n.Date <= end)
                .OrderBy(n => n.Date)
                .ToListAsync();
        }

        /// <summary>Daily returns tagged with the date of the later record (for benchmark alignment).</summary>
        private static List<(DateTime Date, decimal Return)> DatedReturns(IReadOnlyList<NavRecord> ordered)
        {
            var result = new List<(DateTime, decimal)>();
            for (int i = 1; i < ordered.Count; i++)
            {
                var prev = ordered[i - 1].Value;
                var curr = ordered[i].Value;
                decimal r = prev == 0m ? 0m : curr / prev - 1m;
                result.Add((ordered[i].Date, r));
            }
            return result;
        }
    }
}