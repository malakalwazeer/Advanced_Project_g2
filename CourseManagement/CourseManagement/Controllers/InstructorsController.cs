using CourseManagement.ViewModels;
using CourseManagementAPI.Data;
using CourseManagementAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace CourseManagement.Controllers;

//[Authorize(Roles = "Admin,Coordinator")]
[Authorize(Roles = "Admin,TrainingCoordinator")] //malak
public class InstructorsController : Controller
{
    private readonly CourseManagementDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public InstructorsController(CourseManagementDbContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    [HttpGet]
    public async Task<IActionResult> Index(string? searchString)
    {
        var query = _context.Instructors.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(searchString))
        {
            var term = searchString.ToLower();
            query = query.Where(i =>
                i.FullName.ToLower().Contains(term) ||
                i.Email.ToLower().Contains(term) ||
                (i.Qualifications != null && i.Qualifications.ToLower().Contains(term)));
        }

        var instructors = await query
            .Select(i => new InstructorIndexViewModel
            {
                InstructorId   = i.InstructorId,
                FullName       = i.FullName,
                Email          = i.Email,
                Phone          = i.Phone ?? string.Empty,
                Qualifications = i.Qualifications
            })
            .ToListAsync();

        ViewBag.SearchString = searchString;

        return View(instructors);
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View(new InstructorCreateViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(InstructorCreateViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        try
        {
            var identityUser = new ApplicationUser
            {
                UserName = model.Email,
                Email = model.Email,
                EmailConfirmed = true,
                DisplayName = model.FullName
            };

            var identityResult = await _userManager.CreateAsync(identityUser, "Temp123!");

            if (!identityResult.Succeeded)
            {
                foreach (var error in identityResult.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return View(model);
            }

            await _userManager.AddToRoleAsync(identityUser, "Instructor");

            var instructor = new Instructor
            {
                FullName = model.FullName,
                Email = model.Email,
                Phone = model.Phone,
                Qualifications = model.Qualifications,
                Password = "Temp123!"
            };

            _context.Instructors.Add(instructor);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            var innerMessage = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
            ModelState.AddModelError(string.Empty, $"Database Error: {innerMessage}");
            return View(model);
        }
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var instructor = await _context.Instructors.FindAsync(id);
        if (instructor == null)
        {
            return NotFound();
        }

        var model = new InstructorEditViewModel
        {
            InstructorId = instructor.InstructorId,
            FullName = instructor.FullName,
            Email = instructor.Email,
            Phone = instructor.Phone,
            Qualifications = instructor.Qualifications
        };

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, InstructorEditViewModel model)
    {
        if (id != model.InstructorId)
        {
            return NotFound();
        }

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var instructor = await _context.Instructors.FindAsync(id);
        if (instructor == null)
        {
            return NotFound();
        }

        instructor.FullName = model.FullName;
        instructor.Email = model.Email;
        instructor.Phone = model.Phone;
        instructor.Qualifications = model.Qualifications;

        _context.Instructors.Update(instructor);
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Delete(int id)
    {
        var instructor = await _context.Instructors
            .FirstOrDefaultAsync(i => i.InstructorId == id);

        if (instructor == null)
        {
            return NotFound();
        }

        var model = new InstructorIndexViewModel
        {
            InstructorId = instructor.InstructorId,
            FullName = instructor.FullName,
            Email = instructor.Email,
            Phone = instructor.Phone ?? string.Empty,
            Qualifications = instructor.Qualifications
        };

        return View(model);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var instructor = await _context.Instructors.FindAsync(id);
        if (instructor != null)
        {
            var identityUser = await _userManager.FindByEmailAsync(instructor.Email);
            if (identityUser != null)
            {
                await _userManager.DeleteAsync(identityUser);
            }

            _context.Instructors.Remove(instructor);
            await _context.SaveChangesAsync();
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Details(int id)
    {
        var instructor = await _context.Instructors
            .Include(i => i.InstructorExpertises)
                .ThenInclude(ie => ie.Category)
            .Include(i => i.InstructorAvailabilities)
            .FirstOrDefaultAsync(i => i.InstructorId == id);

        if (instructor == null) return NotFound();

        var assignedCategoryIds = instructor.InstructorExpertises.Select(ie => ie.CategoryId).ToList();

        var availableCategories = await _context.CourseCategories
            .Where(c => !assignedCategoryIds.Contains(c.CategoryId))
            .OrderBy(c => c.CategoryName)
            .ToListAsync();

        var model = new InstructorDetailsViewModel
        {
            InstructorId = instructor.InstructorId,
            FullName = instructor.FullName,
            Email = instructor.Email,
            Phone = instructor.Phone ?? string.Empty,
            Qualifications = instructor.Qualifications ?? string.Empty,
            Expertises = instructor.InstructorExpertises.ToList(),
            Availabilities = instructor.InstructorAvailabilities.OrderBy(a => a.AvailableDate).ThenBy(a => a.StartTime).ToList(),
            CategoryList = new SelectList(availableCategories, "CategoryId", "CategoryName")
        };

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddExpertise(InstructorDetailsViewModel model)
    {
        var exists = await _context.InstructorExpertises
            .AnyAsync(ie => ie.InstructorId == model.InstructorId && ie.CategoryId == model.AddCategoryId);

        if (model.AddCategoryId > 0 && !exists)
        {
            var exp = new InstructorExpertise { InstructorId = model.InstructorId, CategoryId = model.AddCategoryId };
            _context.InstructorExpertises.Add(exp);
            await _context.SaveChangesAsync();
        }
        return RedirectToAction(nameof(Details), new { id = model.InstructorId });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> RemoveExpertise(int instructorId, int categoryId)
    {
        var exp = await _context.InstructorExpertises.FindAsync(categoryId, instructorId);
        if (exp != null)
        {
            _context.InstructorExpertises.Remove(exp);
            await _context.SaveChangesAsync();
        }
        return RedirectToAction(nameof(Details), new { id = instructorId });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddAvailability(InstructorDetailsViewModel model)
    {
        if (model.AddStartTime < model.AddEndTime)
        {
            var av = new InstructorAvailability
            {
                InstructorId = model.InstructorId,
                AvailableDate = DateOnly.FromDateTime(model.AddAvailableDate),
                StartTime = TimeOnly.FromTimeSpan(model.AddStartTime),
                EndTime = TimeOnly.FromTimeSpan(model.AddEndTime),
                IsAvailable = true
            };
            _context.InstructorAvailabilities.Add(av);
            await _context.SaveChangesAsync();
        }
        return RedirectToAction(nameof(Details), new { id = model.InstructorId });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> RemoveAvailability(int instructorId, int availabilityId)
    {
        var av = await _context.InstructorAvailabilities.FindAsync(availabilityId);
        if (av != null && av.InstructorId == instructorId)
        {
            _context.InstructorAvailabilities.Remove(av);
            await _context.SaveChangesAsync();
        }
        return RedirectToAction(nameof(Details), new { id = instructorId });
    }
}