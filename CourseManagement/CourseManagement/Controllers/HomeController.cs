using System.Diagnostics;
using CourseManagement.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CourseManagement.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            //if (User.IsInRole("Coordinator"))
            //{
            //    return RedirectToAction("Index", "Dashboard");
            //}
            //return View();

            //malak
            if (User.IsInRole("TrainingCoordinator"))
            {
                return RedirectToAction("Index", "Dashboard");
            }

            if (User.IsInRole("Instructor"))
            {
                return RedirectToAction("Index", "CourseSessions");
            }

            if (User.IsInRole("Trainee"))
            {
                return RedirectToAction("Index", "Courses");
            }

            return View();
        }

        [AllowAnonymous]
        public IActionResult Privacy()
        {
            return View();
        }

        [AllowAnonymous]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
