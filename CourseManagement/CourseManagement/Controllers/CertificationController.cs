using CourseManagement.ViewModels;
using CourseManagementAPI.Data;
using CourseManagementAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace CourseManagement.Controllers;

[Authorize(Roles = "TrainingCoordinator,Instructor,Trainee")]
public class CertificationController : Controller
{
    private readonly CourseManagementDbContext _context;

    public CertificationController(CourseManagementDbContext context)
    {
        _context = context;
    }


    [HttpGet]
    public async Task<IActionResult> Index(string? searchString)
    {
        var query = _context.Certifications
            .Include(c => c.CertificationCourses)
            .AsNoTracking()
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(searchString))
        {
            var term = searchString.ToLower();
            query = query.Where(c => c.Name.ToLower().Contains(term) ||
                                     (c.Description != null && c.Description.ToLower().Contains(term)));
        }

        var vm = await query
            .Select(c => new CertificationIndexViewModel
            {
                CertificationId = c.CertificationId,
                Name            = c.Name,
                Description     = c.Description,
                CourseCount     = c.CertificationCourses.Count
            })
            .ToListAsync();

        ViewBag.SearchString = searchString;
        return View(vm);
    }


    [HttpGet]
    public async Task<IActionResult> Details(int id)
    {
        var cert = await _context.Certifications
            .Include(c => c.CertificationCourses)
                .ThenInclude(cc => cc.Course)
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.CertificationId == id);

        if (cert == null) return NotFound();

        var vm = new CertificationDetailsViewModel
        {
            CertificationId = cert.CertificationId,
            Name            = cert.Name,
            Description     = cert.Description,
            LinkedCourses   = cert.CertificationCourses.Select(cc => new CertificationCourseRow
            {
                CourseId   = cc.CourseId,
                CourseName = cc.Course.CourseName,
                CourseCode = cc.Course.CourseCode,
                IsRequired = cc.IsRequired
            }).OrderBy(r => r.CourseName).ToList()
        };

        if (User.IsInRole("TrainingCoordinator"))
        {
            var linkedIds = cert.CertificationCourses.Select(cc => cc.CourseId).ToHashSet();
            vm.AvailableCourses = new SelectList(
                await _context.Courses.AsNoTracking()
                    .Where(c => !linkedIds.Contains(c.CourseId))
                    .OrderBy(c => c.CourseName)
                    .Select(c => new { c.CourseId, Display = c.CourseCode + " – " + c.CourseName })
                    .ToListAsync(),
                "CourseId", "Display");
        }

        return View(vm);
    }


    [HttpGet]
    [Authorize(Roles = "TrainingCoordinator")]
    public IActionResult Create() => View(new CertificationCreateViewModel());

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "TrainingCoordinator")]
    public async Task<IActionResult> Create(CertificationCreateViewModel model)
    {
        if (!ModelState.IsValid) return View(model);

        var cert = new Certification
        {
            Name        = model.Name.Trim(),
            Description = model.Description?.Trim()
        };

        _context.Certifications.Add(cert);
        await _context.SaveChangesAsync();

        TempData["Success"] = $"Certification \"{cert.Name}\" created.";
        return RedirectToAction(nameof(Details), new { id = cert.CertificationId });
    }


    [HttpGet]
    [Authorize(Roles = "TrainingCoordinator")]
    public async Task<IActionResult> Edit(int id)
    {
        var cert = await _context.Certifications.FindAsync(id);
        if (cert == null) return NotFound();

        return View(new CertificationEditViewModel
        {
            CertificationId = cert.CertificationId,
            Name            = cert.Name,
            Description     = cert.Description
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "TrainingCoordinator")]
    public async Task<IActionResult> Edit(int id, CertificationEditViewModel model)
    {
        if (id != model.CertificationId) return NotFound();
        if (!ModelState.IsValid) return View(model);

        var cert = await _context.Certifications.FindAsync(id);
        if (cert == null) return NotFound();

        cert.Name        = model.Name.Trim();
        cert.Description = model.Description?.Trim();

        await _context.SaveChangesAsync();

        TempData["Success"] = "Certification updated.";
        return RedirectToAction(nameof(Details), new { id });
    }


    [HttpGet]
    [Authorize(Roles = "TrainingCoordinator")]
    public async Task<IActionResult> Delete(int id)
    {
        var cert = await _context.Certifications
            .Include(c => c.CertificationCourses)
            .Include(c => c.TraineeCertificationProgresses)
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.CertificationId == id);

        if (cert == null) return NotFound();

        return View(new CertificationDeleteViewModel
        {
            CertificationId = cert.CertificationId,
            Name            = cert.Name,
            Description     = cert.Description,
            CourseCount     = cert.CertificationCourses.Count,
            ProgressCount   = cert.TraineeCertificationProgresses.Count
        });
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "TrainingCoordinator")]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var cert = await _context.Certifications
            .Include(c => c.CertificationCourses)
            .Include(c => c.TraineeCertificationProgresses)
            .FirstOrDefaultAsync(c => c.CertificationId == id);

        if (cert == null) return NotFound();

        if (cert.TraineeCertificationProgresses.Any())
        {
            ModelState.AddModelError(string.Empty,
                "Cannot delete this certification because trainee progress records are linked to it.");

            return View(new CertificationDeleteViewModel
            {
                CertificationId = cert.CertificationId,
                Name            = cert.Name,
                Description     = cert.Description,
                CourseCount     = cert.CertificationCourses.Count,
                ProgressCount   = cert.TraineeCertificationProgresses.Count
            });
        }

        _context.CertificationCourses.RemoveRange(cert.CertificationCourses);
        _context.Certifications.Remove(cert);
        await _context.SaveChangesAsync();

        TempData["Success"] = $"Certification \"{cert.Name}\" deleted.";
        return RedirectToAction(nameof(Index));
    }


    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "TrainingCoordinator")]
    public async Task<IActionResult> AddCourse(int certificationId, int addCourseId, bool addIsRequired)
    {
        var certExists = await _context.Certifications.AnyAsync(c => c.CertificationId == certificationId);
        if (!certExists) return NotFound();

        var courseExists = await _context.Courses.AnyAsync(c => c.CourseId == addCourseId);
        if (!courseExists || addCourseId == 0)
        {
            TempData["Error"] = "Please select a valid course.";
            return RedirectToAction(nameof(Details), new { id = certificationId });
        }

        var alreadyLinked = await _context.CertificationCourses
            .AnyAsync(cc => cc.CertificationId == certificationId && cc.CourseId == addCourseId);

        if (!alreadyLinked)
        {
            _context.CertificationCourses.Add(new CertificationCourse
            {
                CertificationId = certificationId,
                CourseId        = addCourseId,
                IsRequired      = addIsRequired
            });
            await _context.SaveChangesAsync();
        }

        return RedirectToAction(nameof(Details), new { id = certificationId });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "TrainingCoordinator")]
    public async Task<IActionResult> RemoveCourse(int certificationId, int courseId)
    {
        var link = await _context.CertificationCourses
            .FirstOrDefaultAsync(cc => cc.CertificationId == certificationId && cc.CourseId == courseId);

        if (link != null)
        {
            _context.CertificationCourses.Remove(link);
            await _context.SaveChangesAsync();
        }

        return RedirectToAction(nameof(Details), new { id = certificationId });
    }
}
