using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CourseManagementAPI.Data;
using CourseManagementAPI.Models;
using CourseManagement.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CourseManagement.Controllers
{
    [Authorize(Roles = "Coordinator")]
    public class CoursePrerequisitesController : Controller
    {
        private readonly CourseManagementDbContext _context;

        public CoursePrerequisitesController(CourseManagementDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var prerequisites = await _context.CoursePrerequisites
                .Include(cp => cp.Course)
                .Include(cp => cp.PrerequisiteCourse)
                .Select(cp => new CoursePrerequisiteIndexViewModel
                {
                    CourseId = cp.CourseId,
                    PrerequisiteCourseId = cp.CoursePrerequisiteId,
                    CourseName = cp.Course.CourseName,
                    PrerequisiteCourseName = cp.PrerequisiteCourse.CourseName
                })
                .ToListAsync();

            return View(prerequisites);
        }

        public IActionResult Create()
        {
            ViewData["CourseId"] = new SelectList(_context.Courses, "CourseId", "CourseName");
            ViewData["PrerequisiteCourseId"] = new SelectList(_context.Courses, "CourseId", "CourseName");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CoursePrerequisiteCreateViewModel model)
        {
            if (model.CourseId == model.PrerequisiteCourseId)
            {
                ModelState.AddModelError("", "A course cannot be its own prerequisite.");
            }

            if (ModelState.IsValid)
            {
                bool exists = await _context.CoursePrerequisites
                    .AnyAsync(cp => cp.CourseId == model.CourseId && cp.CoursePrerequisiteId == model.PrerequisiteCourseId);

                if (exists)
                {
                    ModelState.AddModelError("", "This prerequisite relationship already exists.");
                }
                else
                {
                    var prerequisite = new CoursePrerequisite
                    {
                        CourseId = model.CourseId,
                        CoursePrerequisiteId = model.PrerequisiteCourseId
                    };
                    _context.CoursePrerequisites.Add(prerequisite);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
            }

            ViewData["CourseId"] = new SelectList(_context.Courses, "CourseId", "CourseName", model.CourseId);
            ViewData["PrerequisiteCourseId"] = new SelectList(_context.Courses, "CourseId", "CourseName", model.PrerequisiteCourseId);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int courseId, int prerequisiteId)
        {
            var prerequisite = await _context.CoursePrerequisites
                .FirstOrDefaultAsync(cp => cp.CourseId == courseId && cp.CoursePrerequisiteId == prerequisiteId);

            if (prerequisite != null)
            {
                _context.CoursePrerequisites.Remove(prerequisite);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
