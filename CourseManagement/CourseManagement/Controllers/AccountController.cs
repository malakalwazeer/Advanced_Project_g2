using CourseManagement.ViewModels;
using CourseManagementAPI.Data;
using CourseManagementAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CourseManagement.Controllers;

public class AccountController : Controller
{
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;

    private readonly CourseManagementDbContext _context;

    public AccountController(
        SignInManager<ApplicationUser> signInManager,
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager,
        CourseManagementDbContext context)
    {
        _signInManager = signInManager;
        _userManager = userManager;
        _roleManager = roleManager;
        _context = context;
    }

    [HttpGet]
    [AllowAnonymous]
    public IActionResult Login(string? returnUrl = null)
    {
        return View(new LoginViewModel { ReturnUrl = returnUrl });
    }

    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var result = await _signInManager.PasswordSignInAsync(
            model.Email,
            model.Password,
            model.RememberMe,
            lockoutOnFailure: false);

        //if (result.Succeeded)
        //{
        //    return RedirectToLocal(model.ReturnUrl);
        //}

        if (result.Succeeded)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);

            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "User not found.");
                return View(model);
            }

            return await RedirectByRoleAsync(user, model.ReturnUrl);
        }

        ModelState.AddModelError(string.Empty, "Invalid login attempt.");
        return View(model);
    }

    [HttpGet]
    [AllowAnonymous]
    public IActionResult Register()
    {
        return View(new RegisterViewModel());
    }

    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var user = new ApplicationUser
        {
            UserName = model.Email,
            Email = model.Email,
            DisplayName = model.DisplayName
        };

        var result = await _userManager.CreateAsync(user, model.Password);

        if (result.Succeeded)
        {
            // 1. Define your system roles explicitly
            string defaultRole = "Trainee";
            string[] roleNames = { "TrainingCoordinator", "Instructor", "Trainee" };

            // 2. Ensure all application roles exist in the database
            foreach (var roleName in roleNames)
            {
                var roleExist = await _roleManager.RoleExistsAsync(roleName);
                if (!roleExist)
                {
                    await _roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }

            // 3. Assign the newly registered MVC user as a Trainee
            await _userManager.AddToRoleAsync(user, defaultRole);
            var traineeExists = await _context.Trainees
    .AnyAsync(t => t.Email == model.Email);

            if (!traineeExists)
            {
                var trainee = new Trainee
                {
                    FullName = model.DisplayName,
                    Email = model.Email,
                    RegistrationDate = DateOnly.FromDateTime(DateTime.Today),
                    Phone = "Not provided",
                    TraineeStatusId = 1,
                    Password = "Managed by ASP.NET Identity"
                };

                _context.Trainees.Add(trainee);
                await _context.SaveChangesAsync();
            }

            //await _signInManager.SignInAsync(user, isPersistent: false);
            //return RedirectToAction("Index", "Home");

            //malak
            await _signInManager.SignInAsync(user, isPersistent: false);
            return await RedirectByRoleAsync(user);
        }

        foreach (var error in result.Errors)
        {
            ModelState.AddModelError(string.Empty, error.Description);
        }

        return View(model);
    }

    [HttpPost]
    [Authorize]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return RedirectToAction("Index", "Home");
    }

    [HttpGet]
    [AllowAnonymous]
    public IActionResult AccessDenied()
    {
        return View();
    }

    //malak
    private async Task<IActionResult> RedirectByRoleAsync(ApplicationUser user, string? returnUrl = null)
    {
        if (!string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl))
        {
            return Redirect(returnUrl);
        }

        if (await _userManager.IsInRoleAsync(user, "TrainingCoordinator"))
        {
            return RedirectToAction("Index", "Dashboard");
        }

        if (await _userManager.IsInRoleAsync(user, "Instructor"))
        {
            return RedirectToAction("Index", "CourseSessions");
        }

        if (await _userManager.IsInRoleAsync(user, "Trainee"))
        {
            return RedirectToAction("Index", "Courses");
        }

        return RedirectToAction("Index", "Home");
    }
    private IActionResult RedirectToLocal(string? returnUrl)
    {
        if (!string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl))
        {
            return Redirect(returnUrl);
        }

        return RedirectToAction("Index", "Home");
    }
}