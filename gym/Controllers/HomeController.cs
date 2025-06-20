using gym.Data;
using gym.Models;
using gym.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace gym.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IWebHostEnvironment _env;
        private readonly GymContext _context;

        public HomeController(ILogger<HomeController> logger, IWebHostEnvironment env, GymContext context)
        {
            _logger = logger;
            _env = env;
            _context = context;
        }

        public IActionResult CheckEnvironment()
        {
            return Content($"👉 Environment: {_env.EnvironmentName}");
        }
        public IActionResult Index()
        {
            var viewModel = new HomeViewModel
            {
                LatestPackages = _context.Packages
                                         .Where(p => p.IsActive)
                                         .OrderByDescending(p => p.PackageId)
                                         .Take(3)
                                         .ToList(),

                Trainers = _context.Trainers.ToList()
            };

            return View(viewModel);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
