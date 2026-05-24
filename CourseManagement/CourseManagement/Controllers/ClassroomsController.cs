using CourseManagement.ViewModels;
using CourseManagementAPI.Data;
using CourseManagementAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CourseManagement.Controllers;

[Authorize(Roles = "Coordinator")]
public class ClassroomsController : Controller
{
    private readonly CourseManagementDbContext _context;

    public ClassroomsController(CourseManagementDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var classrooms = await _context.Classrooms
            .AsNoTracking()
            .OrderBy(c => c.Location)
            .Select(c => new ClassroomIndexViewModel
            {
                ClassroomId = c.ClassroomId,
                Location = c.Location,
                Capacity = c.Capacity,
                IsActive = c.IsActive
            })
            .ToListAsync();

        return View(classrooms);
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View(new ClassroomCreateViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ClassroomCreateViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var classroom = new Classroom
        {
            Location = model.Location,
            Capacity = model.Capacity,
            IsActive = model.IsActive
        };

        _context.Classrooms.Add(classroom);
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var classroom = await _context.Classrooms
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.ClassroomId == id);

        if (classroom == null)
        {
            return NotFound();
        }

        var model = new ClassroomEditViewModel
        {
            ClassroomId = classroom.ClassroomId,
            Location = classroom.Location,
            Capacity = classroom.Capacity,
            IsActive = classroom.IsActive
        };

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, ClassroomEditViewModel model)
    {
        if (id != model.ClassroomId)
        {
            return NotFound();
        }

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var classroom = await _context.Classrooms.FindAsync(id);

        if (classroom == null)
        {
            return NotFound();
        }

        classroom.Location = model.Location;
        classroom.Capacity = model.Capacity;
        classroom.IsActive = model.IsActive;

        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Delete(int id)
    {
        var classroom = await _context.Classrooms
            .AsNoTracking()
            .Where(c => c.ClassroomId == id)
            .Select(c => new ClassroomIndexViewModel
            {
                ClassroomId = c.ClassroomId,
                Location = c.Location,
                Capacity = c.Capacity,
                IsActive = c.IsActive
            })
            .FirstOrDefaultAsync();

        if (classroom == null)
        {
            return NotFound();
        }

        return View(classroom);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var classroom = await _context.Classrooms.FindAsync(id);

        if (classroom == null)
        {
            return NotFound();
        }

        _context.Classrooms.Remove(classroom);
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }
}
