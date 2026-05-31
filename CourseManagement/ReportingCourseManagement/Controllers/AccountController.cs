using Microsoft.AspNetCore.Mvc;
using ReportingCourseManagement.Dtos;
using ReportingCourseManagement.Services;

namespace ReportingCourseManagement.Controllers
{
    public class AccountController : Controller
    {
        private readonly AuthApiService _authApiService;

        public AccountController(AuthApiService authApiService)
        {
            _authApiService = authApiService;
        }

        [HttpGet]
        public IActionResult Login()
        {
            if (!string.IsNullOrEmpty(HttpContext.Session.GetString("JWToken")))
            {
                return RedirectToAction("Index", "Reports");
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginDto loginDto)
        {
            if (!ModelState.IsValid) return View(loginDto);

            var result = await _authApiService.LoginAsync(loginDto);

            if (result != null && result.IsSuccess)
            {
                // Clean up the incoming string to prevent white-space or casing bugs
                var assignedRole = result.Role?.Trim() ?? string.Empty;

                bool isCoordinator = string.Equals(assignedRole, "TrainingCoordinator", StringComparison.OrdinalIgnoreCase) ||
                                     string.Equals(assignedRole, "Training Coordinator", StringComparison.OrdinalIgnoreCase);

                if (!isCoordinator)
                {
                    string debugMessage = string.IsNullOrEmpty(assignedRole)
                        ? "Access Denied: The API response did not include a user role property."
                        : $"Access Denied: Role '{assignedRole}' is not authorized to view reports.";

                    ModelState.AddModelError(string.Empty, debugMessage);
                    return View(loginDto);
                }

                // Saves values cleanly into session matching ReportApiService's token lookup
                HttpContext.Session.SetString("JWToken", result.Token);
                HttpContext.Session.SetString("UserRole", assignedRole);

                return RedirectToAction("Index", "Reports");
            }

            ModelState.AddModelError(string.Empty, result?.Message ?? "Invalid Login Attempt");
            return View(loginDto);
        }

        [HttpGet]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }
}