using Microsoft.AspNetCore.Mvc;

namespace PerformanceComparator.Controllers
{
    // Placeholder — full comparison UI is built in a later prompt.
    // Exists now so the "Compare" nav link in _Layout resolves instead of 404ing.
    public class CompareController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}