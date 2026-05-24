using CourseManagement.ViewModels;
using CourseManagementAPI.Data;
using CourseManagementAPI.Dtos;
using CourseManagementAPI.Models;
using CourseManagementAPI.Services.Validation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace CourseManagement.Controllers;

[Authorize(Roles = "TrainingCoordinator,Trainee")]
public class EnrollmentsController : Controller
{
    private readonly CourseManagementDbContext _context;
    private readonly EnrollmentValidationService _enrollmentValidator;

    public EnrollmentsController(
        CourseManagementDbContext context,
        EnrollmentValidationService enrollmentValidator)
    {
        _context = context;
        _enrollmentValidator = enrollmentValidator;
    }

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
                .ThenInclude(a => a.Instructor)
            .Include(e => e.Payments)
                .ThenInclude(p => p.PaymentStatus)
            .AsNoTracking()
            .FirstOrDefaultAsync(e => e.EnrollmentId == id);

        if (enrollment == null) return NotFound();
        return View(enrollment);
    }

    public IActionResult Create()
    {
        var vm = new EnrollmentCreateEditViewModel
        {
            EnrollmentDate = DateOnly.FromDateTime(DateTime.Today)
        };
        LoadDropdowns(vm);
        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(EnrollmentCreateEditViewModel vm)
    {
        if (ModelState.IsValid)
        {
            var dto = new CreateEnrollmentDto
            {
                TraineeId = vm.TraineeId,
                SessionId = vm.SessionId
            };

            var error = await _enrollmentValidator.ValidateCreateAsync(dto);
            if (error != null)
            {
                ModelState.AddModelError(string.Empty, error);
                LoadDropdowns(vm);
                return View(vm);
            }

            var enrollment = new Enrollment
            {
                TraineeId          = vm.TraineeId,
                SessionId          = vm.SessionId,
                EnrollmentDate     = vm.EnrollmentDate,
                EnrollmentStatusId = vm.EnrollmentStatusId
            };
            _context.Add(enrollment);
            await _context.SaveChangesAsync();
            TempData["Success"] = "Enrollment created successfully.";
            return RedirectToAction(nameof(Index));
        }
        LoadDropdowns(vm);
        return View(vm);
    }

    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null) return NotFound();

        var enrollment = await _context.Enrollments.FindAsync(id);
        if (enrollment == null) return NotFound();

        var vm = new EnrollmentCreateEditViewModel
        {
            EnrollmentId       = enrollment.EnrollmentId,
            TraineeId          = enrollment.TraineeId,
            SessionId          = enrollment.SessionId,
            EnrollmentDate     = enrollment.EnrollmentDate,
            EnrollmentStatusId = enrollment.EnrollmentStatusId
        };
        LoadDropdowns(vm);
        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, EnrollmentCreateEditViewModel vm)
    {
        if (id != vm.EnrollmentId) return NotFound();

        if (ModelState.IsValid)
        {
            var existing = await _context.Enrollments.AsNoTracking()
                .FirstOrDefaultAsync(e => e.EnrollmentId == id);

            if (existing != null &&
                (existing.TraineeId != vm.TraineeId || existing.SessionId != vm.SessionId))
            {
                var error = await ValidateEnrollmentEdit(vm.TraineeId, vm.SessionId, id);
                if (error != null)
                {
                    ModelState.AddModelError(string.Empty, error);
                    LoadDropdowns(vm);
                    return View(vm);
                }
            }

            var enrollment = new Enrollment
            {
                EnrollmentId       = vm.EnrollmentId,
                TraineeId          = vm.TraineeId,
                SessionId          = vm.SessionId,
                EnrollmentDate     = vm.EnrollmentDate,
                EnrollmentStatusId = vm.EnrollmentStatusId
            };
            try
            {
                _context.Update(enrollment);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Enrollment updated successfully.";
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await EnrollmentExists(vm.EnrollmentId)) return NotFound();
                throw;
            }
            return RedirectToAction(nameof(Index));
        }
        LoadDropdowns(vm);
        return View(vm);
    }

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

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var enrollment = await _context.Enrollments.FindAsync(id);
        if (enrollment != null)
        {
            _context.Enrollments.Remove(enrollment);
            await _context.SaveChangesAsync();
            TempData["Success"] = "Enrollment deleted.";
        }
        return RedirectToAction(nameof(Index));
    }

    private async Task<string?> ValidateEnrollmentEdit(int traineeId, int sessionId, int excludeEnrollmentId)
    {
        var duplicate = await _context.Enrollments.AnyAsync(e =>
            e.TraineeId == traineeId &&
            e.SessionId == sessionId &&
            e.EnrollmentId != excludeEnrollmentId);

        if (duplicate)
            return "This trainee is already enrolled in the selected session.";

        var session = await _context.CourseSessions
            .Include(s => s.Enrollments)
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.SessionId == sessionId);

        if (session == null) return "The selected session does not exist.";

        var enrolledCount = session.Enrollments.Count(e => e.EnrollmentId != excludeEnrollmentId);
        if (enrolledCount >= session.Capacity)
            return "This session has reached its maximum capacity.";

        return null;
    }

    private void LoadDropdowns(EnrollmentCreateEditViewModel vm)
    {
        vm.Trainees = _context.Trainees.AsNoTracking()
            .OrderBy(t => t.FullName)
            .Select(t => new SelectListItem { Value = t.TraineeId.ToString(), Text = t.FullName })
            .ToList();

        vm.Sessions = _context.CourseSessions
            .Include(s => s.Course)
            .AsNoTracking()
            .OrderBy(s => s.StartDateTime)
            .Select(s => new SelectListItem
            {
                Value = s.SessionId.ToString(),
                Text  = s.Course.CourseName + " — " + s.StartDateTime.ToString("yyyy-MM-dd HH:mm")
            })
            .ToList();

        vm.EnrollmentStatuses = _context.EnrollmentStatuses.AsNoTracking()
            .OrderBy(s => s.StatusName)
            .Select(s => new SelectListItem { Value = s.EnrollmentStatusId.ToString(), Text = s.StatusName })
            .ToList();
    }

    private async Task<bool> EnrollmentExists(int id) =>
        await _context.Enrollments.AnyAsync(e => e.EnrollmentId == id);
}
