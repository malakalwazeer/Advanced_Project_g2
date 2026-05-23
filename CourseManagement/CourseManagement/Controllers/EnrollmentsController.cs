using CourseManagementAPI.Data;
using CourseManagementAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace CourseManagement.Controllers;

public class EnrollmentsController : Controller
{
    private readonly CourseManagementDbContext _context;

    public EnrollmentsController(CourseManagementDbContext context)
    {
        _context = context;
    }

    // GET: Enrollments
    public async Task<IActionResult> Index()
    {
        var enrollments = await _context.Enrollments
            .Include(e => e.Trainee)
            .Include(e => e.Session)
                .ThenInclude(s => s.Course)
            .Include(e => e.EnrollmentStatus)
            .AsNoTracking()
            .ToListAsync();

        return View(enrollments);
    }

    // GET: Enrollments/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null) return NotFound();

        var enrollment = await _context.Enrollments
            .Include(e => e.Trainee)
            .Include(e => e.Session)
                .ThenInclude(s => s.Course)
            .Include(e => e.Session)
                .ThenInclude(s => s.Instructor)
            .Include(e => e.EnrollmentStatus)
            .Include(e => e.Assessments)
            .Include(e => e.Payments)
                .ThenInclude(p => p.PaymentStatus)
            .AsNoTracking()
            .FirstOrDefaultAsync(e => e.EnrollmentId == id);

        if (enrollment == null) return NotFound();

        return View(enrollment);
    }

    // GET: Enrollments/Create
    public IActionResult Create()
    {
        LoadDropdowns();
        return View();
    }

    // POST: Enrollments/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(
        [Bind("TraineeId,SessionId,EnrollmentDate,EnrollmentStatusId")]
        Enrollment enrollment)
    {
        if (ModelState.IsValid)
        {
            var validationError = await ValidateEnrollment(enrollment.TraineeId, enrollment.SessionId);
            if (validationError != null)
            {
                ModelState.AddModelError(string.Empty, validationError);
                LoadDropdowns(enrollment.TraineeId, enrollment.SessionId, enrollment.EnrollmentStatusId);
                return View(enrollment);
            }

            _context.Add(enrollment);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        LoadDropdowns(enrollment.TraineeId, enrollment.SessionId, enrollment.EnrollmentStatusId);
        return View(enrollment);
    }

    // GET: Enrollments/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null) return NotFound();

        var enrollment = await _context.Enrollments.FindAsync(id);
        if (enrollment == null) return NotFound();

        LoadDropdowns(enrollment.TraineeId, enrollment.SessionId, enrollment.EnrollmentStatusId);
        return View(enrollment);
    }

    // POST: Enrollments/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id,
        [Bind("EnrollmentId,TraineeId,SessionId,EnrollmentDate,EnrollmentStatusId")]
        Enrollment enrollment)
    {
        if (id != enrollment.EnrollmentId) return NotFound();

        if (ModelState.IsValid)
        {
            var existing = await _context.Enrollments.AsNoTracking()
                .FirstOrDefaultAsync(e => e.EnrollmentId == id);

            // Only re-validate if trainee or session changed
            if (existing != null &&
                (existing.TraineeId != enrollment.TraineeId || existing.SessionId != enrollment.SessionId))
            {
                var validationError = await ValidateEnrollment(
                    enrollment.TraineeId, enrollment.SessionId, excludeEnrollmentId: id);

                if (validationError != null)
                {
                    ModelState.AddModelError(string.Empty, validationError);
                    LoadDropdowns(enrollment.TraineeId, enrollment.SessionId, enrollment.EnrollmentStatusId);
                    return View(enrollment);
                }
            }

            try
            {
                _context.Update(enrollment);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await EnrollmentExists(enrollment.EnrollmentId))
                    return NotFound();
                throw;
            }
            return RedirectToAction(nameof(Index));
        }

        LoadDropdowns(enrollment.TraineeId, enrollment.SessionId, enrollment.EnrollmentStatusId);
        return View(enrollment);
    }

    // GET: Enrollments/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null) return NotFound();

        var enrollment = await _context.Enrollments
            .Include(e => e.Trainee)
            .Include(e => e.Session)
                .ThenInclude(s => s.Course)
            .Include(e => e.EnrollmentStatus)
            .AsNoTracking()
            .FirstOrDefaultAsync(e => e.EnrollmentId == id);

        if (enrollment == null) return NotFound();

        return View(enrollment);
    }

    // POST: Enrollments/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var enrollment = await _context.Enrollments.FindAsync(id);
        if (enrollment != null)
        {
            _context.Enrollments.Remove(enrollment);
            await _context.SaveChangesAsync();
        }
        return RedirectToAction(nameof(Index));
    }

    // ─── Business Rule Helpers ────────────────────────────────────────────────

    /// <summary>
    /// Validates duplicate enrollment, session capacity, and prerequisites.
    /// Returns an error message string, or null if valid.
    /// </summary>
    private async Task<string?> ValidateEnrollment(
        int traineeId, int sessionId, int? excludeEnrollmentId = null)
    {
        // 1. Duplicate enrollment
        var duplicateQuery = _context.Enrollments
            .Where(e => e.TraineeId == traineeId && e.SessionId == sessionId);

        if (excludeEnrollmentId.HasValue)
            duplicateQuery = duplicateQuery.Where(e => e.EnrollmentId != excludeEnrollmentId.Value);

        if (await duplicateQuery.AnyAsync())
            return "This trainee is already enrolled in the selected session.";

        // 2. Session capacity check
        var session = await _context.CourseSessions
            .Include(s => s.Enrollments)
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.SessionId == sessionId);

        if (session == null)
            return "The selected session does not exist.";

        var enrolledCount = session.Enrollments.Count;
        if (excludeEnrollmentId.HasValue)
            enrolledCount = session.Enrollments.Count(e => e.EnrollmentId != excludeEnrollmentId.Value);

        if (enrolledCount >= session.Capacity)
            return "This session has reached its maximum capacity.";

        // 3. Prerequisite validation
        var prerequisites = await _context.CoursePrerequisites
            .Where(cp => cp.CourseId == session.CourseId)
            .AsNoTracking()
            .ToListAsync();

        foreach (var prereq in prerequisites)
        {
            // Trainee must have a passing assessment (Result = 1) in the prerequisite course
            var hasPassed = await _context.Assessments
                .Include(a => a.Enrollment)
                    .ThenInclude(e => e.Session)
                .AnyAsync(a =>
                    a.Enrollment.TraineeId == traineeId &&
                    a.Enrollment.Session.CourseId == prereq.CoursePrerequisiteId &&
                    a.Result == 1);

            if (!hasPassed)
            {
                var prereqCourse = await _context.Courses
                    .AsNoTracking()
                    .FirstOrDefaultAsync(c => c.CourseId == prereq.CoursePrerequisiteId);

                var prereqName = prereqCourse?.CourseName ?? $"Course #{prereq.CoursePrerequisiteId}";
                return $"Prerequisite not met: trainee must pass \"{prereqName}\" before enrolling in this course.";
            }
        }

        return null;
    }

    private void LoadDropdowns(
        int? selectedTraineeId = null,
        int? selectedSessionId = null,
        int? selectedStatusId = null)
    {
        ViewData["TraineeId"] = new SelectList(
            _context.Trainees.AsNoTracking().Select(t => new { t.TraineeId, t.FullName }),
            "TraineeId", "FullName", selectedTraineeId);

        ViewData["SessionId"] = new SelectList(
            _context.CourseSessions
                .Include(s => s.Course)
                .AsNoTracking()
                .Select(s => new
                {
                    s.SessionId,
                    Display = s.Course.CourseName + " — " + s.StartDateTime.ToString("yyyy-MM-dd HH:mm")
                }),
            "SessionId", "Display", selectedSessionId);

        ViewData["EnrollmentStatusId"] = new SelectList(
            _context.EnrollmentStatuses.AsNoTracking(),
            "EnrollmentStatusId", "StatusName", selectedStatusId);
    }

    private async Task<bool> EnrollmentExists(int id) =>
        await _context.Enrollments.AnyAsync(e => e.EnrollmentId == id);
}
