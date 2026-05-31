using CourseManagement.Services;
using CourseManagement.ViewModels;
using CourseManagementAPI.Data;
using CourseManagementAPI.Dtos;
using CourseManagementAPI.Models;
using CourseManagementAPI.Services.Validation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace CourseManagement.Controllers;

//[Authorize(Roles = "Coordinator,Trainee")]
[Authorize(Roles = "TrainingCoordinator,Trainee")] //malak
public class EnrollmentsController : Controller
{
    private static readonly string[] ActiveStatuses =
        ["Enrolled", "Confirmed", "Attending"];
    private readonly CourseManagementDbContext _context;
    private readonly EnrollmentValidationService _enrollmentValidator;
    private readonly UserManager<ApplicationUser> _userManager;//malak
    private readonly EnrollmentBroadcastService _broadcastService;



    public EnrollmentsController(
    CourseManagementDbContext context,
    EnrollmentValidationService enrollmentValidator,
    UserManager<ApplicationUser> userManager,
    EnrollmentBroadcastService broadcastService)
    {
        _context = context;
        _enrollmentValidator = enrollmentValidator;
        _userManager = userManager;
        _broadcastService = broadcastService;
    }

    //added by malak 
    //    TrainingCoordinator sees all enrollments
    //Trainee sees only own enrollments
    public async Task<IActionResult> Index()
    {
        var query = _context.Enrollments
            .Include(e => e.Trainee)
            .Include(e => e.Session)
                .ThenInclude(s => s.Course)
            .Include(e => e.EnrollmentStatus)
            .AsNoTracking()
            .AsQueryable();

        if (User.IsInRole("Trainee"))
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var trainee = await _context.Trainees
                .AsNoTracking()
                .FirstOrDefaultAsync(t => t.Email == user.Email);

            if (trainee == null)
            {
                return Forbid();
            }

            query = query.Where(e => e.TraineeId == trainee.TraineeId);
        }

        var enrollments = await query.ToListAsync();

        var vm = enrollments.Select(e => new EnrollmentIndexViewModel
        {
            EnrollmentId = e.EnrollmentId,
            TraineeName = e.Trainee?.FullName,
            CourseName = e.Session?.Course?.CourseName,
            SessionStart = e.Session?.StartDateTime,
            SessionEnd = e.Session?.EndDateTime,
            EnrollmentDate = e.EnrollmentDate,
            StatusName = e.EnrollmentStatus?.StatusName
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

    [Authorize(Roles = "TrainingCoordinator")]//malak
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
    [Authorize(Roles = "TrainingCoordinator")]//malak
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

            // Resolve the course that owns this session, then push real-time
            // enrollment counts to every browser tab viewing that course's Details page.
            var session = await _context.CourseSessions
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.SessionId == vm.SessionId);

            if (session != null)
                await _broadcastService.BroadcastCourseEnrollmentUpdateAsync(session.CourseId);

            TempData["Success"] = "Enrollment created successfully.";
            return RedirectToAction(nameof(Index));
        }

        LoadDropdowns(vm);
        return View(vm);
    }

    [Authorize(Roles = "TrainingCoordinator")]//malak
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
    [Authorize(Roles = "TrainingCoordinator")]//malak
    public async Task<IActionResult> Edit(int id, EnrollmentEditViewModel vm)
    {
        if (id != vm.EnrollmentId) return NotFound();

        if (ModelState.IsValid)
        {
            // Load existing enrollment WITH its status name so we can detect transitions.
            var existing = await _context.Enrollments
                .Include(e => e.EnrollmentStatus)
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.EnrollmentId == id);

            if (existing == null) return NotFound();

            // Resolve the new status name chosen in the form.
            var newStatus = await _context.EnrollmentStatuses
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.EnrollmentStatusId == vm.EnrollmentStatusId);

            bool wasActive  = ActiveStatuses.Contains(existing.EnrollmentStatus?.StatusName ?? "");
            bool willBeActive = newStatus != null && ActiveStatuses.Contains(newStatus.StatusName);
            bool sessionOrTraineeChanged = existing.TraineeId != vm.TraineeId
                                        || existing.SessionId != vm.SessionId;

            if (sessionOrTraineeChanged || (!wasActive && willBeActive))
            {
                var error = await ValidateEnrollmentEdit(
                    vm.TraineeId, vm.SessionId,
                    excludeEnrollmentId: id,
                    newStatusIsActive: willBeActive);

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

                // Broadcast real-time update to all course-details tabs.
                Console.WriteLine($"[SignalR] Edit saved — broadcasting for sessionId={vm.SessionId}");
                var session = await _context.CourseSessions
                    .AsNoTracking()
                    .FirstOrDefaultAsync(s => s.SessionId == vm.SessionId);
                if (session != null)
                    await _broadcastService.BroadcastCourseEnrollmentUpdateAsync(session.CourseId);

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

    [Authorize(Roles = "TrainingCoordinator")]//malak
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
    [Authorize(Roles = "TrainingCoordinator")]//malak
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var enrollment = await _context.Enrollments.FindAsync(id);
        if (enrollment != null)
        {
            var sessionId = enrollment.SessionId;

            _context.Enrollments.Remove(enrollment);
            await _context.SaveChangesAsync();

            // Broadcast real-time update so open course-details tabs reflect the freed seat.
            Console.WriteLine($"[SignalR] Delete saved — broadcasting for sessionId={sessionId}");
            var session = await _context.CourseSessions
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.SessionId == sessionId);
            if (session != null)
                await _broadcastService.BroadcastCourseEnrollmentUpdateAsync(session.CourseId);

            TempData["Success"] = "Enrollment deleted.";
        }
        return RedirectToAction(nameof(Index));
    }

    private async Task<string?> ValidateEnrollmentEdit(
        int traineeId, int sessionId, int excludeEnrollmentId, bool newStatusIsActive)
    {
        var session = await _context.CourseSessions
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.SessionId == sessionId);

        if (session == null) return "The selected session does not exist.";

        if (newStatusIsActive)
        {
            // Block duplicate ACTIVE enrollment for the same trainee + session.
            var duplicate = await _context.Enrollments.AnyAsync(e =>
                e.TraineeId == traineeId &&
                e.SessionId == sessionId &&
                e.EnrollmentId != excludeEnrollmentId &&
                ActiveStatuses.Contains(e.EnrollmentStatus.StatusName));

            if (duplicate)
                return "This trainee already has an active enrollment in the selected session.";

            // Capacity check — count active enrollments excluding the one being edited.
            var activeCount = await _context.Enrollments
                .CountAsync(e => e.SessionId == sessionId
                    && e.EnrollmentId != excludeEnrollmentId
                    && ActiveStatuses.Contains(e.EnrollmentStatus.StatusName));

            if (activeCount >= session.Capacity)
                return "This session has reached its maximum capacity.";
        }
        else
        {
            // For inactive statuses, still prevent a plain duplicate record.
            var duplicate = await _context.Enrollments.AnyAsync(e =>
                e.TraineeId == traineeId &&
                e.SessionId == sessionId &&
                e.EnrollmentId != excludeEnrollmentId);

            if (duplicate)
                return "This trainee is already enrolled in the selected session.";
        }

        return null;
    }

    //added by malak 
    //trainee cannot choose another trainee.
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Trainee")]
    public async Task<IActionResult> EnrollInSession(int sessionId)
    {
        var user = await _userManager.GetUserAsync(User);

        if (user == null)
        {
            return RedirectToAction("Login", "Account");
        }

        var trainee = await _context.Trainees
            .FirstOrDefaultAsync(t => t.Email == user.Email);

        if (trainee == null)
        {
            return Forbid();
        }

        var dto = new CreateEnrollmentDto
        {
            TraineeId = trainee.TraineeId,
            SessionId = sessionId
        };

        var error = await _enrollmentValidator.ValidateCreateAsync(dto);

        if (error != null)
        {
            TempData["Error"] = error;
            return RedirectToAction("Details", "Courses", new
            {
                id = await _context.CourseSessions
                    .Where(s => s.SessionId == sessionId)
                    .Select(s => s.CourseId)
                    .FirstOrDefaultAsync()
            });
        }

        var status = await _context.EnrollmentStatuses
            .FirstOrDefaultAsync(s => s.StatusName == "Enrolled")
            ?? await _context.EnrollmentStatuses.FirstAsync();

        var enrollment = new Enrollment
        {
            TraineeId = trainee.TraineeId,
            SessionId = sessionId,
            EnrollmentDate = DateOnly.FromDateTime(DateTime.Today),
            EnrollmentStatusId = status.EnrollmentStatusId
        };

        _context.Enrollments.Add(enrollment);
        await _context.SaveChangesAsync();
        var session = await _context.CourseSessions
    .AsNoTracking()
    .FirstOrDefaultAsync(s => s.SessionId == sessionId);

        if (session != null)
        {
            await _broadcastService.BroadcastCourseEnrollmentUpdateAsync(session.CourseId);
        }

        TempData["Success"] = "You have enrolled successfully.";
        return RedirectToAction("Index", "Enrollments");
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
