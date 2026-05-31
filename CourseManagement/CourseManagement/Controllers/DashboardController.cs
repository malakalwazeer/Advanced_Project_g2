using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CourseManagement.Controllers
{
    //[Authorize(Roles = "Coordinator")]
    [Authorize(Roles = "TrainingCoordinator")] //malak
    public class DashboardController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
