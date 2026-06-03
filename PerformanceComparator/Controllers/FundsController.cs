using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PerformanceComparator.Data;
using PerformanceComparator.Services;
using PerformanceComparator.ViewModels;

namespace PerformanceComparator.Controllers
{
    // Public-facing funds pages (browsing). Admin CRUD lives in Areas/Admin.
    public class FundsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IReturnCalculator _returns;
        private readonly IRiskCalculator _risk;

        public FundsController(
            ApplicationDbContext context,
            IReturnCalculator returns,
            IRiskCalculator risk)
        {
            _context = context;
            _returns = returns;
            _risk = risk;
        }

        // GET: /Funds
        public async Task<IActionResult> Index()
        {
            var cards = await _context.Funds
                .Include(f => f.AssetClass)
                .OrderBy(f => f.Name)
                .Select(f => new FundCardViewModel
                {
                    Id = f.Id,
                    Name = f.Name,
                    Symbol = f.Symbol,
                    AssetClassName = f.AssetClass.Name,
                    Provider = f.Provider,
                    LogoFileName = f.LogoFileName,
                    IsBenchmark = f.IsBenchmark,
                    NavCount = f.NavRecords.Count,
                    NavDataFrom = f.NavRecords.Min(n => (DateTime?)n.Date),
                    NavDataTo = f.NavRecords.Max(n => (DateTime?)n.Date)
                })
                .ToListAsync();

            var vm = new FundsListViewModel
            {
                Funds = cards,
                AssetClasses = cards.Select(c => c.AssetClassName)
                                    .Distinct()
                                    .OrderBy(n => n)
                                    .ToList()
            };

            return View(vm);
        }

        // GET: /Funds/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var fund = await _context.Funds
                .Include(f => f.AssetClass)
                .Include(f => f.NavRecords)
                .FirstOrDefaultAsync(f => f.Id == id);

            if (fund is null)
                return NotFound();

            var ordered = fund.NavRecords.OrderBy(n => n.Date).ToList();

            var vm = new PublicFundDetailViewModel
            {
                Id = fund.Id,
                Name = fund.Name,
                Symbol = fund.Symbol,
                AssetClassName = fund.AssetClass.Name,
                Provider = fund.Provider,
                Description = fund.Description,
                LogoFileName = fund.LogoFileName,
                Currency = fund.Currency,
                NavCount = ordered.Count,
                NavDataFrom = ordered.Count > 0 ? ordered.First().Date : null,
                NavDataTo = ordered.Count > 0 ? ordered.Last().Date : null,
                HasEnoughData = ordered.Count >= 2
            };

            if (vm.HasEnoughData)
            {
                var dailyReturns = _returns.DailyReturns(ordered);

                vm.CumulativeReturn = _returns.CumulativeReturn(ordered);
                vm.Cagr = _returns.Cagr(ordered);
                vm.Volatility = _risk.Volatility(dailyReturns);
                vm.MaxDrawdown = _risk.MaxDrawdown(ordered);
                vm.Sharpe = _risk.Sharpe(dailyReturns);
                vm.Sortino = _risk.Sortino(dailyReturns);
            }

            return View(vm);
        }
    }
}