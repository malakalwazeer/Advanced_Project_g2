using Microsoft.AspNetCore.Mvc;
using ReportingCourseManagement.Services;
using Microsoft.AspNetCore.Http;
using ReportingCourseManagement.ViewModels;
using ReportingCourseManagement.Dtos;

namespace ReportingCourseManagement.Controllers
{
    public class ReportsController : Controller
    {
        private readonly ReportApiService _reportApiService;

        public ReportsController(ReportApiService reportApiService)
        {
            _reportApiService = reportApiService;
        }

        // Helper method to centralize authorization checks across actions
        private bool IsAuthorized()
        {
            var token = HttpContext.Session.GetString("JWToken");
            var role = HttpContext.Session.GetString("UserRole");

            return !string.IsNullOrEmpty(token) && role == "TrainingCoordinator";
        }

        public async Task<IActionResult> Index()
        {
            if (!IsAuthorized())
            {
                HttpContext.Session.Clear();
                return RedirectToAction("Login", "Account");
            }

            var model = new ReportsDashboardViewModel
            {
                EnrollmentByCourse = await _reportApiService.GetEnrollmentByCourseAsync(),
                EnrollmentByCategory = await _reportApiService.GetEnrollmentByCategoryAsync(),
                InstructorWorkload = await _reportApiService.GetInstructorWorkloadAsync(),
                RevenueSummary = await _reportApiService.GetRevenueSummaryAsync() ?? new RevenueSummaryReportDto(),
                CertificationCompletion = await _reportApiService.GetCertificationCompletionAsync()
            };

            return View(model);
        }

        public async Task<IActionResult> EnrollmentByCourse()
        {
            if (!IsAuthorized())
            {
                HttpContext.Session.Clear();
                return RedirectToAction("Login", "Account");
            }

            var data = await _reportApiService.GetEnrollmentByCourseAsync();
            return View(data);
        }

        public async Task<IActionResult> EnrollmentByCategory()
        {
            if (!IsAuthorized())
            {
                HttpContext.Session.Clear();
                return RedirectToAction("Login", "Account");
            }

            var data = await _reportApiService.GetEnrollmentByCategoryAsync();
            return View(data);
        }

        public async Task<IActionResult> InstructorWorkload()
        {
            if (!IsAuthorized())
            {
                HttpContext.Session.Clear();
                return RedirectToAction("Login", "Account");
            }

            var data = await _reportApiService.GetInstructorWorkloadAsync();
            return View(data);
        }

        public async Task<IActionResult> RevenueSummary()
        {
            if (!IsAuthorized())
            {
                HttpContext.Session.Clear();
                return RedirectToAction("Login", "Account");
            }

            var data = await _reportApiService.GetRevenueSummaryAsync();
            return View(data);
        }

        public async Task<IActionResult> CertificationCompletion()
        {
            if (!IsAuthorized())
            {
                HttpContext.Session.Clear();
                return RedirectToAction("Login", "Account");
            }

            var data = await _reportApiService.GetCertificationCompletionAsync();
            return View(data);
        }
    }
}