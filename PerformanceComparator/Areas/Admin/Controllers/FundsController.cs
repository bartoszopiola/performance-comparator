using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PerformanceComparator.Data;
using PerformanceComparator.Models;
using PerformanceComparator.Services;
using PerformanceComparator.ViewModels.Admin;

namespace PerformanceComparator.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class FundsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ICsvNavImporter _navImporter;
        private readonly IWebHostEnvironment _env;

        // Allowed image extensions + content types for logo upload
        private static readonly string[] AllowedLogoExtensions = [".png", ".jpg", ".jpeg", ".webp"];
        private static readonly string[] AllowedLogoContentTypes =
            ["image/png", "image/jpeg", "image/jpg", "image/webp"];

        private const long MaxLogoBytes = 2 * 1024 * 1024; // 2 MB
        private const long MaxCsvBytes = 5 * 1024 * 1024; // 5 MB

        public FundsController(
            ApplicationDbContext context,
            ICsvNavImporter navImporter,
            IWebHostEnvironment env)
        {
            _context = context;
            _navImporter = navImporter;
            _env = env;
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
                    NavRecordCount = f.NavRecords.Count,
                    LogoFileName = f.LogoFileName
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
                NavDataTo = fund.NavRecords.Any() ? fund.NavRecords.Max(n => n.Date) : null,
                LogoFileName = fund.LogoFileName
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
            // Validate logo if one was provided
            if (vm.LogoFile is not null && !ValidateLogo(vm.LogoFile, out var logoError))
                ModelState.AddModelError(nameof(vm.LogoFile), logoError);

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

            // Save logo file (if any)
            if (vm.LogoFile is not null)
                entity.LogoFileName = await SaveLogoAsync(vm.LogoFile);

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
                ExistingLogoFileName = entity.LogoFileName,
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

            if (vm.LogoFile is not null && !ValidateLogo(vm.LogoFile, out var logoError))
                ModelState.AddModelError(nameof(vm.LogoFile), logoError);

            if (!ModelState.IsValid)
            {
                vm.AssetClasses = await GetAssetClassSelectListAsync();
                vm.ExistingLogoFileName = (await _context.Funds.AsNoTracking()
                    .FirstOrDefaultAsync(f => f.Id == id))?.LogoFileName;
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

            // New logo uploaded → save new, delete old
            if (vm.LogoFile is not null)
            {
                var oldLogo = entity.LogoFileName;
                entity.LogoFileName = await SaveLogoAsync(vm.LogoFile);

                if (!string.IsNullOrEmpty(oldLogo))
                    DeleteLogo(oldLogo);
            }

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
                AssetClassName = fund.AssetClass.Name,
                LogoFileName = fund.LogoFileName
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

            var logo = entity.LogoFileName;

            _context.Funds.Remove(entity);
            await _context.SaveChangesAsync();

            // Remove the logo file after the row is gone
            if (!string.IsNullOrEmpty(logo))
                DeleteLogo(logo);

            return RedirectToAction(nameof(Index));
        }

        // ── NAV CSV upload ───────────────────────────────────────────────────────

        // GET: /Admin/Funds/UploadNav/5
        public async Task<IActionResult> UploadNav(int id)
        {
            var fund = await _context.Funds.FindAsync(id);
            if (fund is null)
                return NotFound();

            var vm = new UploadNavViewModel
            {
                FundId = fund.Id,
                FundName = fund.Name,
                Symbol = fund.Symbol,
                ExistingNavCount = await _context.NavRecords.CountAsync(n => n.FundId == id)
            };

            return View(vm);
        }

        // POST: /Admin/Funds/UploadNav/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UploadNav(int id, IFormFile file)
        {
            var fund = await _context.Funds.FindAsync(id);
            if (fund is null)
                return NotFound();

            if (file is null || file.Length == 0)
            {
                TempData["Error"] = "Please choose a CSV file to upload.";
                return RedirectToAction(nameof(UploadNav), new { id });
            }

            // Validate extension (never trust content type alone, but check both)
            var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (ext != ".csv")
            {
                TempData["Error"] = "Only .csv files are allowed.";
                return RedirectToAction(nameof(UploadNav), new { id });
            }

            if (file.Length > MaxCsvBytes)
            {
                TempData["Error"] = "File is too large (max 5 MB).";
                return RedirectToAction(nameof(UploadNav), new { id });
            }

            // Read into a stream — never use the uploaded filename for anything on disk
            await using var stream = file.OpenReadStream();
            var result = await _navImporter.ImportAsync(id, stream);

            if (result.Added == 0 && result.HasErrors)
            {
                TempData["Error"] = "Import failed: " + string.Join("; ", result.Errors.Take(5));
            }
            else
            {
                var msg = $"Import complete. Added: {result.Added}, Skipped (duplicates): {result.Skipped}.";
                if (result.HasErrors)
                    msg += $" {result.Errors.Count} row error(s) — first: {result.Errors.First()}";
                TempData["Success"] = msg;
            }

            return RedirectToAction(nameof(UploadNav), new { id });
        }

        // ── Helpers ──────────────────────────────────────────────────────────────

        private async Task<SelectList> GetAssetClassSelectListAsync()
        {
            var items = await _context.AssetClasses
                .OrderBy(a => a.Name)
                .ToListAsync();

            return new SelectList(items, "Id", "Name");
        }

        /// <summary>Validates extension AND content type AND size.</summary>
        private static bool ValidateLogo(IFormFile file, out string error)
        {
            error = string.Empty;

            var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!AllowedLogoExtensions.Contains(ext))
            {
                error = "Logo must be a .png, .jpg, .jpeg or .webp file.";
                return false;
            }

            if (!AllowedLogoContentTypes.Contains(file.ContentType.ToLowerInvariant()))
            {
                error = "Logo content type is not a supported image format.";
                return false;
            }

            if (file.Length > MaxLogoBytes)
            {
                error = "Logo is too large (max 2 MB).";
                return false;
            }

            return true;
        }

        /// <summary>Saves the logo with a server-generated GUID filename. Returns the filename.</summary>
        private async Task<string> SaveLogoAsync(IFormFile file)
        {
            var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
            var safeName = $"{Guid.NewGuid():N}{ext}";

            var dir = Path.Combine(_env.WebRootPath, "uploads", "logos");
            Directory.CreateDirectory(dir);

            var fullPath = Path.Combine(dir, safeName);
            await using (var fs = new FileStream(fullPath, FileMode.Create))
            {
                await file.CopyToAsync(fs);
            }

            return safeName;
        }

        /// <summary>Deletes a logo file from disk if it exists.</summary>
        private void DeleteLogo(string fileName)
        {
            var fullPath = Path.Combine(_env.WebRootPath, "uploads", "logos", fileName);
            if (System.IO.File.Exists(fullPath))
                System.IO.File.Delete(fullPath);
        }
    }
}