using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PerformanceComparator.Data;
using PerformanceComparator.ViewModels.Admin;

namespace PerformanceComparator.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class DashboardController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DashboardController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var vm = new DashboardViewModel
            {
                FundCount = await _context.Funds.CountAsync(),
                AssetClassCount = await _context.AssetClasses.CountAsync(),
                ContentBlockCount = await _context.ContentBlocks.CountAsync()
            };

            return View(vm);
        }
    }
}