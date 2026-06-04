using PerformanceComparator.Models;
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PerformanceComparator.Data;
using PerformanceComparator.ViewModels;

namespace PerformanceComparator.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var blocks = await _context.ContentBlocks
                .Where(c => c.Key == "home.hero" || c.Key == "home.intro")
                .ToDictionaryAsync(c => c.Key);

            var vm = new HomeViewModel();

            if (blocks.TryGetValue("home.hero", out var hero))
            {
                if (!string.IsNullOrWhiteSpace(hero.Title)) vm.HeroTitle = hero.Title;
                if (!string.IsNullOrWhiteSpace(hero.Body)) vm.HeroBody = hero.Body;
            }

            if (blocks.TryGetValue("home.intro", out var intro))
            {
                if (!string.IsNullOrWhiteSpace(intro.Title)) vm.IntroTitle = intro.Title;
                if (!string.IsNullOrWhiteSpace(intro.Body)) vm.IntroBody = intro.Body;
            }

            return View(vm);
        }

        public async Task<IActionResult> About()
        {
            var block = await _context.ContentBlocks
                .FirstOrDefaultAsync(c => c.Key == "about.body");

            var vm = new AboutViewModel();
            if (block is not null)
            {
                if (!string.IsNullOrWhiteSpace(block.Title)) vm.Title = block.Title;
                if (!string.IsNullOrWhiteSpace(block.Body)) vm.Body = block.Body;
            }

            return View(vm);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        // Friendly handler for non-success status codes (e.g. 404).
        // Wired via UseStatusCodePagesWithReExecute in Program.cs.
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Status(int code)
        {
            ViewBag.Code = code;
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel
            {
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
            });
        }
    }
}