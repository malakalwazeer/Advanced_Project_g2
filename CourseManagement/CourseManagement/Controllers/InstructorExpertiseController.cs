using CourseManagement.ViewModels;
using CourseManagementAPI.Data;
using CourseManagementAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace CourseManagement.Controllers
{
    [Authorize(Roles = "Coordinator")]
    public class InstructorExpertiseController : Controller
    {
        private readonly CourseManagementDbContext _context;

        public InstructorExpertiseController(CourseManagementDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var expertise = await _context.InstructorExpertises
                .Include(e => e.Instructor)
                .Include(e => e.Category)
                .OrderBy(e => e.Instructor.FullName)
                .ThenBy(e => e.Category.CategoryName)
                .Select(e => new ExpertiseIndexViewModel
                {
                    InstructorId = e.InstructorId,
                    CategoryId = e.CategoryId,
                    InstructorName = e.Instructor.FullName,
                    CategoryName = e.Category.CategoryName
                })
                .ToListAsync();

            return View(expertise);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            await PopulateSelectListsAsync();
            return View(new ExpertiseCreateViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ExpertiseCreateViewModel model)
        {
            // Check for duplicate
            bool exists = await _context.InstructorExpertises
                .AnyAsync(e => e.InstructorId == model.InstructorId && e.CategoryId == model.CategoryId);
            
            if (exists)
            {
                ModelState.AddModelError("", "This instructor is already linked to this category.");
            }

            if (!ModelState.IsValid)
            {
                await PopulateSelectListsAsync(model.InstructorId, model.CategoryId);
                return View(model);
            }

            var expertise = new InstructorExpertise
            {
                InstructorId = model.InstructorId,
                CategoryId = model.CategoryId
            };

            _context.InstructorExpertises.Add(expertise);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int instructorId, int categoryId)
        {
            var expertise = await _context.InstructorExpertises
                .FirstOrDefaultAsync(e => e.InstructorId == instructorId && e.CategoryId == categoryId);

            if (expertise != null)
            {
                _context.InstructorExpertises.Remove(expertise);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private async Task PopulateSelectListsAsync(int? selectedInstructorId = null, int? selectedCategoryId = null)
        {
            var instructors = await _context.Instructors
                .OrderBy(i => i.FullName)
                .ToListAsync();

            var categories = await _context.CourseCategories
                .OrderBy(c => c.CategoryName)
                .ToListAsync();

            ViewBag.Instructors = new SelectList(
                instructors,
                nameof(Instructor.InstructorId),
                nameof(Instructor.FullName),
                selectedInstructorId);

            ViewBag.Categories = new SelectList(
                categories,
                nameof(CourseCategory.CategoryId),
                nameof(CourseCategory.CategoryName),
                selectedCategoryId);
        }
    }
}
