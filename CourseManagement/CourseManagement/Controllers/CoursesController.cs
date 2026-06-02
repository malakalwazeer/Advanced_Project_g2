using CourseManagement.ViewModels;
using CourseManagementAPI.Data;
using CourseManagementAPI.Models;
using CourseManagementAPI.Dtos;
using CourseManagementAPI.Services.Validation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CourseManagement.Services;

namespace CourseManagement.Controllers
{
    //[Authorize(Roles = "Admin,Coordinator")]
    [Authorize(Roles = "TrainingCoordinator,Trainee")] //malak
    public class CoursesController : Controller
    {
        private readonly CourseManagementDbContext _context;
        private readonly CourseValidationService _validationService;
        private readonly EnrollmentBroadcastService _broadcastService;

        public CoursesController(
            CourseManagementDbContext context,
            CourseValidationService validationService,
            EnrollmentBroadcastService broadcastService)
        {
            _context = context;
            _validationService = validationService;
            _broadcastService = broadcastService;
        }

        [HttpGet]
        public async Task<IActionResult> Index(string? searchString, int? categoryId)
        {
            var query = _context.Courses
                .Include(c => c.Category)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchString))
            {
                var term = searchString.ToLower();
                query = query.Where(c =>
                    c.CourseName.ToLower().Contains(term) ||
                    c.CourseCode.ToLower().Contains(term) ||
                    (c.Description != null && c.Description.ToLower().Contains(term)));
            }

            if (categoryId.HasValue)
                query = query.Where(c => c.CategoryId == categoryId.Value);

            var courses = await query
                .Select(c => new CourseIndexViewModel
                {
                    CourseId     = c.CourseId,
                    CourseCode   = c.CourseCode,
                    Title        = c.CourseName,
                    CategoryName = c.Category.CategoryName,
                    Fee          = c.EnrollmentFee
                })
                .ToListAsync();

            ViewBag.SearchString = searchString;
            ViewBag.CategoryId   = categoryId;
            ViewBag.Categories   = new SelectList(
                await _context.CourseCategories.AsNoTracking()
                    .OrderBy(c => c.CategoryName)
                    .ToListAsync(),
                "CategoryId", "CategoryName", categoryId);

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

            //malak added available sessions
            var model = new CourseDetailsViewModel
            {
                CourseId = course.CourseId,
                CourseCode = course.CourseCode,
                CourseName = course.CourseName,
                Description = course.Description,
                DurationHours = course.DurationHours,
                Capacity = course.Capacity,
                CurrentRequirements = course.CourseReqEquipments.ToList(),
                CurrentPrerequisites = course.CoursePrerequisites.ToList(),
                EquipmentList = new SelectList(
    await _context.Equipments
        .OrderBy(e => e.EquipmentName)
        .ToListAsync(),
    "EquipmentId",
    "EquipmentName"),

                PrerequisiteCourseList = new SelectList(
    await _context.Courses
        .Where(c => c.CourseId != id)
        .OrderBy(c => c.CourseName)
        .ToListAsync(),
    "CourseId",
    "CourseName"),
            Sessions = await _broadcastService.GetSessionSnapshotsAsync(id),

            AvailableSessions = await _context.CourseSessions
    .Include(s => s.Course)
    .Include(s => s.Instructor)
    .Include(s => s.Classroom)
    .Where(s => s.CourseId == id)
    .Select(s => new SessionIndexViewModel
    {
        SessionId = s.SessionId,
        CourseName = s.Course.CourseName,
        InstructorName = s.Instructor.FullName,
        ClassroomLocation = s.Classroom.Location,
        StartDateTime = s.StartDateTime,
        EndDateTime = s.EndDateTime,
        Capacity = s.Capacity
    })
    .ToListAsync()

            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "TrainingCoordinator")] //malak
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
        [Authorize(Roles = "TrainingCoordinator")] //malak
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
        [Authorize(Roles = "TrainingCoordinator")] //malak
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
        [Authorize(Roles = "TrainingCoordinator")] //malak
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
        [Authorize(Roles = "TrainingCoordinator")] //malak
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
        [Authorize(Roles = "TrainingCoordinator")] //malak
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
                        DurationHours = model.DurationHours,
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
        [Authorize(Roles = "TrainingCoordinator")] //malak
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
        [Authorize(Roles = "TrainingCoordinator")] //malak
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
                course.DurationHours = model.DurationHours;
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
        [Authorize(Roles = "TrainingCoordinator")] //malak
        public async Task<IActionResult> Delete(int id)
        {
            var course = await _context.Courses
                .Include(c => c.Category)
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
        [Authorize(Roles = "TrainingCoordinator")] //malak
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
