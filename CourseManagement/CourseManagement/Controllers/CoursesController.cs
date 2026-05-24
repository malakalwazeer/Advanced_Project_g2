using CourseManagement.ViewModels;
using CourseManagementAPI.Data;
using CourseManagementAPI.Models;
using CourseManagementAPI.Dtos;
using CourseManagementAPI.Services.Validation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace CourseManagement.Controllers
{
    [Authorize(Roles = "Admin,Coordinator")]
    public class CoursesController : Controller
    {
        private readonly CourseManagementDbContext _context;
        private readonly CourseValidationService _validationService;

        public CoursesController(CourseManagementDbContext context, CourseValidationService validationService)
        {
            _context = context;
            _validationService = validationService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var courses = await _context.Courses
                .Include(c => c.Category)
                .AsNoTracking()
                .Select(c => new CourseIndexViewModel
                {
                    CourseId = c.CourseId,
                    CourseCode = c.CourseCode,
                    Title = c.CourseName,
                    CategoryName = c.Category.CategoryName,
                    Fee = c.EnrollmentFee
                })
                .ToListAsync();

            return View(courses);
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var course = await _context.Courses
                .Include(c => c.Category)
                .Include(c => c.CoursePrerequisites)
                    .ThenInclude(cp => cp.PrerequisiteCourse)
                .Include(c => c.CourseReqEquipments)
                    .ThenInclude(cre => cre.Equipment)
                .FirstOrDefaultAsync(c => c.CourseId == id);

            if (course == null) return NotFound();

            var model = new CourseDetailsViewModel
            {
                CourseId = course.CourseId,
                CourseCode = course.CourseCode,
                CourseName = course.CourseName,
                Description = course.Description,
                Capacity = course.Capacity,
                CurrentRequirements = course.CourseReqEquipments.ToList(),
                CurrentPrerequisites = course.CoursePrerequisites.ToList(),
                EquipmentList = new SelectList(await _context.Equipments.OrderBy(e => e.EquipmentName).ToListAsync(), "EquipmentId", "EquipmentName"),
                PrerequisiteCourseList = new SelectList(await _context.Courses.Where(c => c.CourseId != id).OrderBy(c => c.CourseName).ToListAsync(), "CourseId", "CourseName")
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddEquipmentRequirement(CourseDetailsViewModel model)
        {
            if (model.AddEquipmentId > 0 && model.AddEquipmentQuantity > 0)
            {
                var existingReq = await _context.CourseReqEquipments
                    .FirstOrDefaultAsync(r => r.CourseId == model.CourseId && r.EquipmentId == model.AddEquipmentId);

                if (existingReq != null)
                {
                    existingReq.Quantity = model.AddEquipmentQuantity;
                }
                else
                {
                    var req = new CourseReqEquipment { CourseId = model.CourseId, EquipmentId = model.AddEquipmentId, Quantity = model.AddEquipmentQuantity };
                    _context.CourseReqEquipments.Add(req);
                }
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Details), new { id = model.CourseId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveEquipmentRequirement(int courseId, int equipmentId)
        {
            var req = await _context.CourseReqEquipments.FindAsync(equipmentId, courseId);
            if (req != null)
            {
                _context.CourseReqEquipments.Remove(req);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Details), new { id = courseId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddPrerequisite(CourseDetailsViewModel model)
        {
            if (model.AddPrerequisiteId > 0)
            {
                var existingPre = await _context.CoursePrerequisites
                    .FirstOrDefaultAsync(p => p.CourseId == model.CourseId && p.CoursePrerequisiteId == model.AddPrerequisiteId);
                
                if (existingPre == null)
                {
                    var pre = new CoursePrerequisite { CourseId = model.CourseId, CoursePrerequisiteId = model.AddPrerequisiteId };
                    _context.CoursePrerequisites.Add(pre);
                    await _context.SaveChangesAsync();
                }
            }
            return RedirectToAction(nameof(Details), new { id = model.CourseId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemovePrerequisite(int courseId, int prerequisiteId)
        {
            var pre = await _context.CoursePrerequisites.FindAsync(courseId, prerequisiteId);
            if (pre != null)
            {
                _context.CoursePrerequisites.Remove(pre);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Details), new { id = courseId });
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var model = new CourseCreateViewModel
            {
                Categories = await GetCategorySelectListAsync()
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CourseCreateViewModel model)
        {
            if (ModelState.IsValid)
            {
                var dto = new CreateCourseDto
                {
                    CourseCode = model.CourseCode,
                    CourseName = model.CourseName,
                    CategoryId = model.CategoryId
                };

                string? error = await _validationService.ValidateCreateAsync(dto);
                if (error != null)
                {
                    ModelState.AddModelError(string.Empty, error);
                }
                else
                {
                    var course = new Course
                    {
                        CourseCode = model.CourseCode,
                        CourseName = model.CourseName,
                        Description = model.Description,
                        Capacity = model.Capacity,
                        EnrollmentFee = model.EnrollmentFee,
                        CategoryId = model.CategoryId
                    };

                    _context.Courses.Add(course);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
            }

            model.Categories = await GetCategorySelectListAsync(model.CategoryId);
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var course = await _context.Courses.FindAsync(id);
            if (course == null) return NotFound();

            var model = new CourseEditViewModel
            {
                CourseId = course.CourseId,
                CourseCode = course.CourseCode,
                CourseName = course.CourseName,
                Description = course.Description,
                DurationHours = course.DurationHours, 
                Capacity = course.Capacity,
                EnrollmentFee = course.EnrollmentFee,
                CategoryId = course.CategoryId,
                Categories = await GetCategorySelectListAsync(course.CategoryId)
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, CourseEditViewModel model)
        {
            if (id != model.CourseId) return NotFound();

            if (ModelState.IsValid)
            {
                var course = await _context.Courses.FindAsync(id);
                if (course == null) return NotFound();

                course.CourseCode = model.CourseCode;
                course.CourseName = model.CourseName;
                course.Description = model.Description;
                course.DurationHours = course.DurationHours;
                course.Capacity = model.Capacity;
                course.EnrollmentFee = model.EnrollmentFee;
                course.CategoryId = model.CategoryId;

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            model.Categories = await GetCategorySelectListAsync(model.CategoryId);
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var course = await _context.Courses
                .Include(c => c.Category)
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.CourseId == id);

            if (course == null) return NotFound();

            var model = new CourseIndexViewModel
            {
                CourseId = course.CourseId,
                CourseCode = course.CourseCode,
                Title = course.CourseName,
                CategoryName = course.Category.CategoryName,
                Fee = course.EnrollmentFee
            };

            return View(model);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var course = await _context.Courses
                .Include(c => c.CoursePrerequisites)
                .Include(c => c.RequiredForCourses)
                .Include(c => c.CourseReqEquipments)
                .Include(c => c.CourseSessions)
                .FirstOrDefaultAsync(c => c.CourseId == id);

            if (course == null) return NotFound();

            
            // Validation to stop FK errors - check if course is saved somewhere else
            List<string> reasons = new List<string>();

            if (course.CourseSessions.Any()) reasons.Add("It has active course sessions.");
            if (course.CourseReqEquipments.Any()) reasons.Add("It has equipment requirements defined.");
            if (course.CoursePrerequisites.Any() || course.RequiredForCourses.Any())
                reasons.Add("It is linked as a prerequisite for/by other courses.");

            if (reasons.Any())
            {
                ModelState.AddModelError(string.Empty, "Cannot delete this course because: " + string.Join(" ", reasons));
                
                var model = new CourseIndexViewModel
                {
                    CourseId = course.CourseId,
                    CourseCode = course.CourseCode,
                    Title = course.CourseName
                };
                return View("Delete", model);
            }

            _context.Courses.Remove(course);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
        private async Task<SelectList> GetCategorySelectListAsync(int? selectedCategoryId = null)
        {
            var categories = await _context.CourseCategories
                .AsNoTracking()
                .OrderBy(c => c.CategoryName)
                .ToListAsync();

            return new SelectList(
                categories,
                nameof(CourseCategory.CategoryId),
                nameof(CourseCategory.CategoryName),
                selectedCategoryId);
        }
    }
}
