using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PerformanceComparator.Data;
using PerformanceComparator.Services;
using PerformanceComparator.ViewModels;

namespace PerformanceComparator.Controllers
{
    public class CompareController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IComparisonService _comparison;

        public CompareController(ApplicationDbContext context, IComparisonService comparison)
        {
            _context = context;
            _comparison = comparison;
        }

        // GET: /Compare
        public async Task<IActionResult> Index()
        {
            var vm = await BuildFormViewModelAsync();
            return View(vm);
        }

        // POST: /Compare/Results
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Results(CompareRequestViewModel request)
        {
            if (request.FundIds is null || request.FundIds.Length == 0)
                ModelState.AddModelError("FundIds", "Select at least one fund.");

            if (request.FundIds?.Length > 4)
                ModelState.AddModelError("FundIds", "You can compare at most 4 funds.");

            if (request.Start >= request.End)
                ModelState.AddModelError("End", "End date must be after start date.");

            if (!ModelState.IsValid)
            {
                ViewBag.FormError = true;
                return View("Index", await BuildFormViewModelAsync());
            }

            // User enters risk-free rate as % (e.g. 2), service expects decimal (0.02)
            decimal rfDecimal = request.RiskFreeRate / 100m;

            var result = await _comparison.CompareAsync(
                request.FundIds!,
                request.BenchmarkId,
                request.Start,
                request.End,
                rfDecimal);

            return View("Results", result);
        }

        private async Task<CompareFormViewModel> BuildFormViewModelAsync()
        {
            var funds = await _context.Funds
                .OrderBy(f => f.Name)
                .ToListAsync();

            var items = funds
                .Select(f => new SelectListItem($"{f.Name} ({f.Symbol})", f.Id.ToString()))
                .ToList();

            return new CompareFormViewModel
            {
                AllFunds = items,
                BenchmarkFunds = items,
                DefaultStart = DateTime.Today.AddYears(-3),
                DefaultEnd = DateTime.Today,
                DefaultRiskFreeRate = 2m
            };
        }
    }
}