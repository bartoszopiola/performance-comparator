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
    public class ContentBlocksController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ContentBlocksController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: /Admin/ContentBlocks
        public async Task<IActionResult> Index()
        {
            var items = await _context.ContentBlocks
                .OrderBy(c => c.Key)
                .Select(c => new ContentBlockListItemViewModel
                {
                    Id = c.Id,
                    Key = c.Key,
                    Title = c.Title,
                    UpdatedAt = c.UpdatedAt
                })
                .ToListAsync();

            return View(items);
        }

        // GET: /Admin/ContentBlocks/Create
        public IActionResult Create()
        {
            return View(new ContentBlockFormViewModel());
        }

        // POST: /Admin/ContentBlocks/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ContentBlockFormViewModel vm)
        {
            if (!ModelState.IsValid)
                return View(vm);

            // Enforce unique key
            if (await _context.ContentBlocks.AnyAsync(c => c.Key == vm.Key))
            {
                ModelState.AddModelError(nameof(vm.Key), "A content block with this key already exists.");
                return View(vm);
            }

            _context.ContentBlocks.Add(new ContentBlock
            {
                Key = vm.Key,
                Title = vm.Title,
                Body = vm.Body,
                UpdatedAt = DateTime.UtcNow
            });
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // GET: /Admin/ContentBlocks/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var entity = await _context.ContentBlocks.FindAsync(id);
            if (entity is null)
                return NotFound();

            var vm = new ContentBlockFormViewModel
            {
                Id = entity.Id,
                Key = entity.Key,
                Title = entity.Title,
                Body = entity.Body,
                UpdatedAt = entity.UpdatedAt
            };

            return View(vm);
        }

        // POST: /Admin/ContentBlocks/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ContentBlockFormViewModel vm)
        {
            if (id != vm.Id)
                return NotFound();

            if (!ModelState.IsValid)
                return View(vm);

            var entity = await _context.ContentBlocks.FindAsync(id);
            if (entity is null)
                return NotFound();

            // Key is the stable identifier — keep it unchanged on edit.
            entity.Title = vm.Title;
            entity.Body = vm.Body;
            entity.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // GET: /Admin/ContentBlocks/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var entity = await _context.ContentBlocks.FirstOrDefaultAsync(c => c.Id == id);
            if (entity is null)
                return NotFound();

            var vm = new ContentBlockFormViewModel
            {
                Id = entity.Id,
                Key = entity.Key,
                Title = entity.Title
            };

            return View(vm);
        }

        // POST: /Admin/ContentBlocks/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var entity = await _context.ContentBlocks.FindAsync(id);
            if (entity is null)
                return NotFound();

            _context.ContentBlocks.Remove(entity);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}