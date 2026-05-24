using Microsoft.AspNetCore.Mvc;
using ReportingCourseManagement.Services;

namespace ReportingCourseManagement.Controllers
{
    public class ReportsController : Controller
    {
        //This controller will call ReportApiService and send the data to the views.
        private readonly ReportApiService _reportApiService;

        public ReportsController(ReportApiService reportApiService)
        {
            _reportApiService = reportApiService;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> EnrollmentByCourse()
        {
            var data = await _reportApiService.GetEnrollmentByCourseAsync();
            return View(data);
        }

        public async Task<IActionResult> EnrollmentByCategory()
        {
            var data = await _reportApiService.GetEnrollmentByCategoryAsync();
            return View(data);
        }

        public async Task<IActionResult> InstructorWorkload()
        {
            var data = await _reportApiService.GetInstructorWorkloadAsync();
            return View(data);
        }

        public async Task<IActionResult> RevenueSummary()
        {
            var data = await _reportApiService.GetRevenueSummaryAsync();
            return View(data);
        }

        public async Task<IActionResult> CertificationCompletion()
        {
            var data = await _reportApiService.GetCertificationCompletionAsync();
            return View(data);
        }
    }
}
