using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PerformanceComparator.Data;
using PerformanceComparator.Models;
using PerformanceComparator.ViewModels.Admin;

namespace PerformanceComparator.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class AssetClassesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AssetClassesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: /Admin/AssetClasses
        public async Task<IActionResult> Index()
        {
            var items = await _context.AssetClasses
                .OrderBy(a => a.Name)
                .Select(a => new AssetClassFormViewModel
                {
                    Id = a.Id,
                    Name = a.Name
                })
                .ToListAsync();

            return View(items);
        }

        // GET: /Admin/AssetClasses/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var entity = await _context.AssetClasses
                .FirstOrDefaultAsync(a => a.Id == id);

            if (entity is null)
                return NotFound();

            var vm = new AssetClassFormViewModel
            {
                Id = entity.Id,
                Name = entity.Name
            };

            return View(vm);
        }

        // GET: /Admin/AssetClasses/Create
        public IActionResult Create()
        {
            return View(new AssetClassFormViewModel());
        }

        // POST: /Admin/AssetClasses/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AssetClassFormViewModel vm)
        {
            if (!ModelState.IsValid)
                return View(vm);

            var entity = new AssetClass
            {
                Name = vm.Name
            };

            _context.AssetClasses.Add(entity);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // GET: /Admin/AssetClasses/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var entity = await _context.AssetClasses.FindAsync(id);
            if (entity is null)
                return NotFound();

            var vm = new AssetClassFormViewModel
            {
                Id = entity.Id,
                Name = entity.Name
            };

            return View(vm);
        }

        // POST: /Admin/AssetClasses/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, AssetClassFormViewModel vm)
        {
            if (id != vm.Id)
                return NotFound();

            if (!ModelState.IsValid)
                return View(vm);

            var entity = await _context.AssetClasses.FindAsync(id);
            if (entity is null)
                return NotFound();

            entity.Name = vm.Name;
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // GET: /Admin/AssetClasses/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var entity = await _context.AssetClasses
                .FirstOrDefaultAsync(a => a.Id == id);

            if (entity is null)
                return NotFound();

            // Check if any funds reference this asset class
            var fundCount = await _context.Funds.CountAsync(f => f.AssetClassId == id);

            var vm = new AssetClassFormViewModel
            {
                Id = entity.Id,
                Name = entity.Name
            };

            if (fundCount > 0)
                ViewBag.CannotDelete = $"Cannot delete: {fundCount} fund(s) are assigned to this asset class. Reassign or delete them first.";

            return View(vm);
        }

        // POST: /Admin/AssetClasses/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var entity = await _context.AssetClasses.FindAsync(id);
            if (entity is null)
                return NotFound();

            // Prevent deletion if funds reference this asset class
            var fundCount = await _context.Funds.CountAsync(f => f.AssetClassId == id);
            if (fundCount > 0)
            {
                var vm = new AssetClassFormViewModel { Id = entity.Id, Name = entity.Name };
                ViewBag.CannotDelete = $"Cannot delete: {fundCount} fund(s) are assigned to this asset class. Reassign or delete them first.";
                return View("Delete", vm);
            }

            _context.AssetClasses.Remove(entity);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}