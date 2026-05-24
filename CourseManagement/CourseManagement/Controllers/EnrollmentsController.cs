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

        var vm = enrollments.Select(e => new EnrollmentIndexViewModel
        {
            EnrollmentId   = e.EnrollmentId,
            TraineeName    = e.Trainee?.FullName,
            CourseName     = e.Session?.Course?.CourseName,
            SessionStart   = e.Session?.StartDateTime,
            SessionEnd     = e.Session?.EndDateTime,
            EnrollmentDate = e.EnrollmentDate,
            StatusName     = e.EnrollmentStatus?.StatusName
        }).ToList();

        return View(vm);
    }

    public async Task<IActionResult> Details(int? id)
    {
        if (id == null) return NotFound();

        var e = await _context.Enrollments
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

        if (e == null) return NotFound();

        var vm = new EnrollmentDetailsViewModel
        {
            EnrollmentId   = e.EnrollmentId,
            TraineeName    = e.Trainee?.FullName,
            CourseName     = e.Session?.Course?.CourseName,
            InstructorName = e.Session?.Instructor?.FullName,
            SessionStart   = e.Session?.StartDateTime,
            SessionEnd     = e.Session?.EndDateTime,
            EnrollmentDate = e.EnrollmentDate,
            StatusName     = e.EnrollmentStatus?.StatusName,
            Payments = e.Payments.Select(p => new EnrollmentPaymentRow
            {
                AmountPaid       = p.AmountPaid,
                PaymentDate      = p.PaymentDate,
                BalanceRemaining = p.BalanceRemaining,
                StatusName       = p.PaymentStatus?.StatusName
            }).ToList(),
            Assessments = e.Assessments.Select(a => new EnrollmentAssessmentRow
            {
                InstructorName = a.Instructor?.FullName,
                Score          = a.Score,
                Result         = a.Result
            }).ToList()
        };

        return View(vm);
    }

    public IActionResult Create()
    {
        var vm = new EnrollmentCreateViewModel
        {
            EnrollmentDate = DateOnly.FromDateTime(DateTime.Today)
        };
        LoadDropdowns(vm);
        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(EnrollmentCreateViewModel vm)
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

        var enrollment = await _context.Enrollments.AsNoTracking()
            .FirstOrDefaultAsync(e => e.EnrollmentId == id);
        if (enrollment == null) return NotFound();

        var vm = new EnrollmentEditViewModel
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
    public async Task<IActionResult> Edit(int id, EnrollmentEditViewModel vm)
    {
        if (id != vm.EnrollmentId) return NotFound();

        if (ModelState.IsValid)
        {
            var existing = await _context.Enrollments.AsNoTracking()
                .FirstOrDefaultAsync(e => e.EnrollmentId == id);

            if (existing != null &&
                (existing.TraineeId != vm.TraineeId || existing.SessionId != vm.SessionId))
            {
                var error = await ValidateEnrollmentEdit(vm.TraineeId, vm.SessionId, excludeEnrollmentId: id);
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

        var e = await _context.Enrollments
            .Include(e => e.Trainee)
            .Include(e => e.Session)
                .ThenInclude(s => s.Course)
            .Include(e => e.EnrollmentStatus)
            .AsNoTracking()
            .FirstOrDefaultAsync(e => e.EnrollmentId == id);

        if (e == null) return NotFound();

        var vm = new EnrollmentDeleteViewModel
        {
            EnrollmentId   = e.EnrollmentId,
            TraineeName    = e.Trainee?.FullName,
            CourseName     = e.Session?.Course?.CourseName,
            SessionStart   = e.Session?.StartDateTime,
            EnrollmentDate = e.EnrollmentDate,
            StatusName     = e.EnrollmentStatus?.StatusName
        };

        return View(vm);
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

    private void LoadDropdowns(EnrollmentCreateViewModel vm)
    {
        vm.Trainees = _context.Trainees.AsNoTracking()
            .OrderBy(t => t.FullName)
            .Select(t => new SelectListItem { Value = t.TraineeId.ToString(), Text = t.FullName })
            .ToList();

        vm.Sessions = _context.CourseSessions
            .Include(s => s.Course)
            .AsNoTracking()
            .OrderBy(s => s.StartDateTime)
            .AsEnumerable()
            .Select(s => new SelectListItem
            {
                Value = s.SessionId.ToString(),
                Text  = s.Course.CourseName + " — " + s.StartDateTime.ToString("yyyy-MM-dd HH:mm")
            }).ToList();

        vm.EnrollmentStatuses = _context.EnrollmentStatuses.AsNoTracking()
            .Select(s => new SelectListItem { Value = s.EnrollmentStatusId.ToString(), Text = s.StatusName })
            .ToList();
    }

    private void LoadDropdowns(EnrollmentEditViewModel vm)
    {
        vm.Trainees = _context.Trainees.AsNoTracking()
            .OrderBy(t => t.FullName)
            .Select(t => new SelectListItem { Value = t.TraineeId.ToString(), Text = t.FullName })
            .ToList();

        vm.Sessions = _context.CourseSessions
            .Include(s => s.Course)
            .AsNoTracking()
            .OrderBy(s => s.StartDateTime)
            .AsEnumerable()
            .Select(s => new SelectListItem
            {
                Value = s.SessionId.ToString(),
                Text  = s.Course.CourseName + " — " + s.StartDateTime.ToString("yyyy-MM-dd HH:mm")
            }).ToList();

        vm.EnrollmentStatuses = _context.EnrollmentStatuses.AsNoTracking()
            .Select(s => new SelectListItem { Value = s.EnrollmentStatusId.ToString(), Text = s.StatusName })
            .ToList();
    }

    private async Task<bool> EnrollmentExists(int id) =>
        await _context.Enrollments.AnyAsync(e => e.EnrollmentId == id);
}
