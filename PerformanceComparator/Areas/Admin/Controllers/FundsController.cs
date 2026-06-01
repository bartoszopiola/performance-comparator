using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PerformanceComparator.Data;
using PerformanceComparator.Models;
using PerformanceComparator.ViewModels.Admin;

namespace PerformanceComparator.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class FundsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public FundsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: /Admin/Funds
        public async Task<IActionResult> Index()
        {
            var funds = await _context.Funds
                .Include(f => f.AssetClass)
                .OrderBy(f => f.Name)
                .Select(f => new FundIndexItemViewModel
                {
                    Id = f.Id,
                    Name = f.Name,
                    Symbol = f.Symbol,
                    AssetClassName = f.AssetClass.Name,
                    Provider = f.Provider,
                    Currency = f.Currency,
                    IsBenchmark = f.IsBenchmark,
                    NavRecordCount = f.NavRecords.Count
                })
                .ToListAsync();

            var vm = new FundIndexViewModel { Funds = funds };
            return View(vm);
        }

        // GET: /Admin/Funds/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var fund = await _context.Funds
                .Include(f => f.AssetClass)
                .Include(f => f.NavRecords)
                .FirstOrDefaultAsync(f => f.Id == id);

            if (fund is null)
                return NotFound();

            var vm = new FundDetailViewModel
            {
                Id = fund.Id,
                Name = fund.Name,
                Symbol = fund.Symbol,
                AssetClassName = fund.AssetClass.Name,
                Provider = fund.Provider,
                Description = fund.Description,
                Currency = fund.Currency,
                IsBenchmark = fund.IsBenchmark,
                CreatedAt = fund.CreatedAt,
                NavRecordCount = fund.NavRecords.Count,
                NavDataFrom = fund.NavRecords.Any() ? fund.NavRecords.Min(n => n.Date) : null,
                NavDataTo = fund.NavRecords.Any() ? fund.NavRecords.Max(n => n.Date) : null
            };

            return View(vm);
        }

        // GET: /Admin/Funds/Create
        public async Task<IActionResult> Create()
        {
            var vm = new FundFormViewModel
            {
                AssetClasses = await GetAssetClassSelectListAsync()
            };

            return View(vm);
        }

        // POST: /Admin/Funds/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(FundFormViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                vm.AssetClasses = await GetAssetClassSelectListAsync();
                return View(vm);
            }

            var entity = new Fund
            {
                Name = vm.Name,
                Symbol = vm.Symbol,
                AssetClassId = vm.AssetClassId,
                Provider = vm.Provider,
                Description = vm.Description,
                Currency = vm.Currency,
                IsBenchmark = vm.IsBenchmark,
                CreatedAt = DateTime.UtcNow
            };

            _context.Funds.Add(entity);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // GET: /Admin/Funds/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var entity = await _context.Funds.FindAsync(id);
            if (entity is null)
                return NotFound();

            var vm = new FundFormViewModel
            {
                Id = entity.Id,
                Name = entity.Name,
                Symbol = entity.Symbol,
                AssetClassId = entity.AssetClassId,
                Provider = entity.Provider,
                Description = entity.Description,
                Currency = entity.Currency,
                IsBenchmark = entity.IsBenchmark,
                AssetClasses = await GetAssetClassSelectListAsync()
            };

            return View(vm);
        }

        // POST: /Admin/Funds/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, FundFormViewModel vm)
        {
            if (id != vm.Id)
                return NotFound();

            if (!ModelState.IsValid)
            {
                vm.AssetClasses = await GetAssetClassSelectListAsync();
                return View(vm);
            }

            var entity = await _context.Funds.FindAsync(id);
            if (entity is null)
                return NotFound();

            entity.Name = vm.Name;
            entity.Symbol = vm.Symbol;
            entity.AssetClassId = vm.AssetClassId;
            entity.Provider = vm.Provider;
            entity.Description = vm.Description;
            entity.Currency = vm.Currency;
            entity.IsBenchmark = vm.IsBenchmark;

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // GET: /Admin/Funds/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var fund = await _context.Funds
                .Include(f => f.AssetClass)
                .FirstOrDefaultAsync(f => f.Id == id);

            if (fund is null)
                return NotFound();

            var navCount = await _context.NavRecords.CountAsync(n => n.FundId == id);

            var vm = new FundDetailViewModel
            {
                Id = fund.Id,
                Name = fund.Name,
                Symbol = fund.Symbol,
                AssetClassName = fund.AssetClass.Name
            };

            if (navCount > 0)
                ViewBag.NavWarning = $"This will also delete {navCount} NAV record(s).";

            return View(vm);
        }

        // POST: /Admin/Funds/Delete/5
        // NavRecords are cascade-deleted by EF Core (configured in OnModelCreating).
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var entity = await _context.Funds.FindAsync(id);
            if (entity is null)
                return NotFound();

            _context.Funds.Remove(entity);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // ── Helper ────────────────────────────────────────────────────────────
        private async Task<SelectList> GetAssetClassSelectListAsync()
        {
            var items = await _context.AssetClasses
                .OrderBy(a => a.Name)
                .ToListAsync();

            return new SelectList(items, "Id", "Name");
        }
    }
}