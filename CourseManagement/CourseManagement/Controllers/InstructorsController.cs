using CourseManagement.ViewModels;
using CourseManagementAPI.Data;
using CourseManagementAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace CourseManagement.Controllers;

//[Authorize(Roles = "Admin,Coordinator")]
[Authorize(Roles = "TrainingCoordinator")] //malak
public class InstructorsController : Controller
{
    private readonly CourseManagementDbContext _context;

    public InstructorsController(CourseManagementDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var instructors = await _context.Instructors
            .AsNoTracking()
            .Select(i => new InstructorIndexViewModel
            {
                InstructorId = i.InstructorId,
                FullName = i.FullName,
                Email = i.Email,
                Phone = i.Phone ?? string.Empty,
                Qualifications = i.Qualifications
            })
            .ToListAsync();
        return View(instructors);
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

        // Get IDs of already assigned categories to exclude them from dropdown
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
        // Double check to prevent duplicate key error
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
