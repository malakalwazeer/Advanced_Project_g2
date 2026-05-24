using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CourseManagementAPI.Data;
using CourseManagementAPI.Models;
using CourseManagementAPI.Dtos;
using CourseManagementAPI.Services.Validation;
using CourseManagement.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CourseManagement.Controllers
{
    [Authorize(Roles = "Coordinator")]
    public class CourseSessionsController : Controller
    {
        private readonly CourseManagementDbContext _context;
        private readonly CourseSessionValidationService _validationService;

        public CourseSessionsController(CourseManagementDbContext context, CourseSessionValidationService validationService)
        {
            _context = context;
            _validationService = validationService;
        }

        public async Task<IActionResult> Index()
        {
            var sessions = await _context.CourseSessions
                .Include(s => s.Course)
                .Include(s => s.Instructor)
                .Include(s => s.Classroom)
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
                .ToListAsync();

            return View(sessions);
        }

        public IActionResult Create()
        {
            PopulateSelectLists();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SessionCreateViewModel model)
        {
            if (ModelState.IsValid)
            {
                var dto = new CreateCourseSessionDto
                {
                    CourseId = model.CourseId,
                    InstructorId = model.InstructorId,
                    ClassroomId = model.ClassroomId,
                    StartDateTime = model.StartDateTime,
                    EndDateTime = model.EndDateTime,
                    Capacity = model.Capacity
                };

                string? error = await _validationService.ValidateCreateAsync(dto);
                if (error != null)
                {
                    ModelState.AddModelError(string.Empty, error);
                }
                else
                {
                    var session = new CourseSession
                    {
                        CourseId = model.CourseId,
                        InstructorId = model.InstructorId,
                        ClassroomId = model.ClassroomId,
                        StartDateTime = model.StartDateTime,
                        EndDateTime = model.EndDateTime,
                        Capacity = model.Capacity,
                        CreatedAt = DateTime.Now
                    };

                    _context.CourseSessions.Add(session);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
            }

            PopulateSelectLists(model.CourseId, model.InstructorId, model.ClassroomId);
            return View(model);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var session = await _context.CourseSessions.FindAsync(id);
            if (session == null) return NotFound();

            var model = new SessionEditViewModel
            {
                SessionId = session.SessionId,
                CourseId = session.CourseId,
                InstructorId = session.InstructorId,
                ClassroomId = session.ClassroomId,
                StartDateTime = session.StartDateTime,
                EndDateTime = session.EndDateTime,
                Capacity = session.Capacity
            };

            PopulateSelectLists(model.CourseId, model.InstructorId, model.ClassroomId);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, SessionEditViewModel model)
        {
            if (id != model.SessionId) return NotFound();

            if (ModelState.IsValid)
            {
                var session = await _context.CourseSessions
                    .Include(s => s.Course)
                    .FirstOrDefaultAsync(s => s.SessionId == id);
                if (session == null) return NotFound();

                session.CourseId = model.CourseId;
                session.InstructorId = model.InstructorId;
                session.ClassroomId = model.ClassroomId;
                session.StartDateTime = model.StartDateTime;
                session.EndDateTime = model.EndDateTime;
                session.Capacity = model.Capacity;

                _context.Notifications.Add(new Notification { Title = "Session Updated", Message = "A session was modified.", SessionId = model.SessionId });
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            PopulateSelectLists(model.CourseId, model.InstructorId, model.ClassroomId);
            return View(model);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var session = await _context.CourseSessions
                .Include(s => s.Course)
                .Include(s => s.Instructor)
                .Include(s => s.Classroom)
                .FirstOrDefaultAsync(m => m.SessionId == id);

            if (session == null) return NotFound();

            var model = new SessionIndexViewModel
            {
                SessionId = session.SessionId,
                CourseName = session.Course.CourseName,
                InstructorName = session.Instructor.FullName,
                ClassroomLocation = session.Classroom.Location,
                StartDateTime = session.StartDateTime,
                EndDateTime = session.EndDateTime,
                Capacity = session.Capacity
            };

            return View(model);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var session = await _context.CourseSessions
                .Include(s => s.Enrollments)
                .Include(s => s.Notifications)
                .FirstOrDefaultAsync(s => s.SessionId == id);

            if (session == null) return NotFound();

            List<string> reasons = new List<string>();

            if (session.Enrollments.Any()) reasons.Add("It has enrollments.");
            if (session.Notifications.Any()) reasons.Add("It has notifications associated with it.");

            if (reasons.Any())
            {
                ModelState.AddModelError(string.Empty, "Cannot delete this session because: " + string.Join(" ", reasons));
                
                var model = await _context.CourseSessions
                    .Include(s => s.Course)
                    .Include(s => s.Instructor)
                    .Include(s => s.Classroom)
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
                    .FirstOrDefaultAsync(m => m.SessionId == id);

                return View("Delete", model);
            }

            _context.CourseSessions.Remove(session);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private void PopulateSelectLists(int? courseId = null, int? instructorId = null, int? classroomId = null)
        {
            ViewData["CourseId"] = new SelectList(_context.Courses, "CourseId", "CourseName", courseId);
            ViewData["InstructorId"] = new SelectList(_context.Instructors, "InstructorId", "FullName", instructorId);
            ViewData["ClassroomId"] = new SelectList(_context.Classrooms, "ClassroomId", "Location", classroomId);
        }
    }
}
