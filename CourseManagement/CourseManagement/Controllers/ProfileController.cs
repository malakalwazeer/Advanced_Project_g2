using CourseManagement.ViewModels;
using CourseManagementAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace CourseManagement.Controllers;

[Authorize] // Accessible by any logged-in user (TrainingCoordinator, Instructor, Trainee)
public class ProfileController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;

    public ProfileController(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager)
    {
        _userManager = userManager;
        _signInManager = signInManager;
    }

    [HttpGet]
    public async Task<IActionResult> Settings()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return NotFound("User could not be loaded.");
        }

        var roles = await _userManager.GetRolesAsync(user);

        var model = new ProfileSettingsViewModel
        {
            Email = user.Email ?? string.Empty,
            DisplayName = user.DisplayName ?? user.UserName ?? string.Empty,
            ActiveRole = roles.FirstOrDefault() ?? "No Assigned Role"
        };

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Settings(ProfileSettingsViewModel model)
    {
        // Fetch current user details again to re-populate information cards if view reloads
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return NotFound("User could not be loaded.");
        }

        var roles = await _userManager.GetRolesAsync(user);
        model.Email = user.Email ?? string.Empty;
        model.DisplayName = user.DisplayName ?? user.UserName ?? string.Empty;
        model.ActiveRole = roles.FirstOrDefault() ?? "No Assigned Role";

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        // Change password using Identity engine (saves directly to DB)
        var changePasswordResult = await _userManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);

        if (!changePasswordResult.Succeeded)
        {
            foreach (var error in changePasswordResult.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
            return View(model);
        }

        // Refresh sign-in cookie context for the user so their current session doesn't expire
        await _signInManager.RefreshSignInAsync(user);

        TempData["SuccessMessage"] = "Your password has been updated successfully!";
        return RedirectToAction(nameof(Settings));
    }
}