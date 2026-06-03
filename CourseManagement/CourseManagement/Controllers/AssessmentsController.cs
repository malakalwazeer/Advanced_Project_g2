using CourseManagement.Services;
using CourseManagement.ViewModels;
using CourseManagementAPI.Data;
using CourseManagementAPI.Dtos;
using CourseManagementAPI.Models;
using CourseManagementAPI.Services.Validation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace CourseManagement.Controllers;

[Authorize(Roles = "TrainingCoordinator,Instructor,Trainee")]
public class AssessmentsController : Controller
{
    private readonly CourseManagementDbContext _context;
    private readonly AssessmentValidationService _assessmentValidator;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly CertificationProgressService _progressService;

    public AssessmentsController(
        CourseManagementDbContext context,
        AssessmentValidationService assessmentValidator,
        UserManager<ApplicationUser> userManager,
        CertificationProgressService progressService)
    {
        _context = context;
        _assessmentValidator = assessmentValidator;
        _userManager = userManager;
        _progressService = progressService;
    }


    public async Task<IActionResult> Index(string? searchString, int? resultFilter)
    {
        var query = _context.Assessments
            .Include(a => a.Enrollment)
                .ThenInclude(e => e.Trainee)
            .Include(a => a.Enrollment)
                .ThenInclude(e => e.Session)
                    .ThenInclude(s => s.Course)
            .Include(a => a.Instructor)
            .AsNoTracking()
            .AsQueryable();

        // Role-based restriction applied first so search cannot bypass it
        if (User.IsInRole("Trainee"))
        {
            var user = await _userManager.GetUserAsync(User);
            var trainee = await _context.Trainees.AsNoTracking()
                .FirstOrDefaultAsync(t => t.Email == user!.Email);
            if (trainee == null) return Forbid();
            query = query.Where(a => a.Enrollment.TraineeId == trainee.TraineeId);
        }
        else if (User.IsInRole("Instructor") && !User.IsInRole("TrainingCoordinator"))
        {
            var user = await _userManager.GetUserAsync(User);
            var instructor = await _context.Instructors.AsNoTracking()
                .FirstOrDefaultAsync(i => i.Email == user!.Email);
            if (instructor == null) return Forbid();
            query = query.Where(a => a.Enrollment.Session.InstructorId == instructor.InstructorId);
        }

        if (!string.IsNullOrWhiteSpace(searchString))
        {
            var term = searchString.ToLower();
            query = query.Where(a =>
                a.Enrollment.Trainee.FullName.ToLower().Contains(term) ||
                a.Enrollment.Session.Course.CourseName.ToLower().Contains(term) ||
                a.Instructor.FullName.ToLower().Contains(term));
        }

        // resultFilter: 1 = Pass, 0 = Fail, -1 = Not Graded (null result)
        if (resultFilter.HasValue)
        {
            if (resultFilter.Value == -1)
                query = query.Where(a => a.Result == null);
            else
                query = query.Where(a => a.Result == resultFilter.Value);
        }

        var assessments = await query.ToListAsync();

        var vm = assessments.Select(a => new AssessmentIndexViewModel
        {
            AssessmentId   = a.AssessmentId,
            TraineeName    = a.Enrollment?.Trainee?.FullName,
            CourseName     = a.Enrollment?.Session?.Course?.CourseName,
            InstructorName = a.Instructor?.FullName,
            Score          = a.Score,
            Result         = a.Result
        }).ToList();

        ViewBag.SearchString  = searchString;
        ViewBag.ResultFilter  = resultFilter;

        return View(vm);
    }


    public async Task<IActionResult> Details(int? id)
    {
        if (id == null) return NotFound();

        var a = await _context.Assessments
            .Include(a => a.Enrollment)
                .ThenInclude(e => e.Trainee)
            .Include(a => a.Enrollment)
                .ThenInclude(e => e.Session)
                    .ThenInclude(s => s.Course)
            .Include(a => a.Instructor)
            .AsNoTracking()
            .FirstOrDefaultAsync(a => a.AssessmentId == id);

        if (a == null) return NotFound();

        if (User.IsInRole("Trainee"))
        {
            var user = await _userManager.GetUserAsync(User);
            var trainee = await _context.Trainees.AsNoTracking()
                .FirstOrDefaultAsync(t => t.Email == user!.Email);
            if (trainee == null || a.Enrollment?.TraineeId != trainee.TraineeId)
                return Forbid();
        }
        else if (User.IsInRole("Instructor") && !User.IsInRole("TrainingCoordinator"))
        {
            var user = await _userManager.GetUserAsync(User);
            var instructor = await _context.Instructors.AsNoTracking()
                .FirstOrDefaultAsync(i => i.Email == user!.Email);
            if (instructor == null || a.Enrollment?.Session?.InstructorId != instructor.InstructorId)
                return Forbid();
        }

        var vm = new AssessmentDetailsViewModel
        {
            AssessmentId   = a.AssessmentId,
            TraineeName    = a.Enrollment?.Trainee?.FullName,
            CourseName     = a.Enrollment?.Session?.Course?.CourseName,
            SessionStart   = a.Enrollment?.Session?.StartDateTime,
            InstructorName = a.Instructor?.FullName,
            Score          = a.Score,
            Result         = a.Result
        };

        return View(vm);
    }


    [Authorize(Roles = "TrainingCoordinator,Instructor")]
    public async Task<IActionResult> Create()
    {
        var vm = new AssessmentCreateViewModel();

        // Pre-populate InstructorId so the hidden field submits the right value
        if (User.IsInRole("Instructor") && !User.IsInRole("TrainingCoordinator"))
        {
            var user = await _userManager.GetUserAsync(User);
            var instructor = await _context.Instructors.AsNoTracking()
                .FirstOrDefaultAsync(i => i.Email == user!.Email);
            if (instructor == null) return Forbid();
            vm.InstructorId = instructor.InstructorId;
        }

        await LoadDropdownsAsync(vm);
        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "TrainingCoordinator,Instructor")]
    public async Task<IActionResult> Create(AssessmentCreateViewModel vm)
    {
        if (vm.Result.HasValue && vm.Result != 0 && vm.Result != 1)
            ModelState.AddModelError(string.Empty, "Result must be 0 (Fail) or 1 (Pass).");

        if (User.IsInRole("Instructor") && !User.IsInRole("TrainingCoordinator"))
        {
            var scopeError = await ValidateInstructorScopeAsync(vm.EnrollmentId, vm.InstructorId);
            if (scopeError != null)
                ModelState.AddModelError(string.Empty, scopeError);
        }

        if (ModelState.IsValid)
        {
            var dto = new CreateAssessmentDto
            {
                EnrollmentId = vm.EnrollmentId,
                InstructorId = vm.InstructorId,
                Result       = vm.Result ?? 0,
                Score        = vm.Score
            };

            var error = await _assessmentValidator.ValidateCreateAsync(dto);
            if (error != null)
            {
                ModelState.AddModelError(string.Empty, error);
                await LoadDropdownsAsync(vm);
                return View(vm);
            }

            var assessment = new Assessment
            {
                EnrollmentId = vm.EnrollmentId,
                InstructorId = vm.InstructorId,
                Score        = vm.Score,
                Result       = vm.Result
            };

            _context.Add(assessment);
            await _context.SaveChangesAsync();
            await _progressService.RecalculateFromEnrollmentAsync(assessment.EnrollmentId);

            TempData["Success"] = "Assessment saved. Certification progress updated.";
            return RedirectToAction(nameof(Index));
        }

        await LoadDropdownsAsync(vm);
        return View(vm);
    }


    [Authorize(Roles = "TrainingCoordinator,Instructor")]
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null) return NotFound();

        var assessment = await _context.Assessments
            .Include(a => a.Enrollment)
                .ThenInclude(e => e.Session)
            .AsNoTracking()
            .FirstOrDefaultAsync(a => a.AssessmentId == id);
        if (assessment == null) return NotFound();

        if (User.IsInRole("Instructor") && !User.IsInRole("TrainingCoordinator"))
        {
            var user = await _userManager.GetUserAsync(User);
            var instructor = await _context.Instructors.AsNoTracking()
                .FirstOrDefaultAsync(i => i.Email == user!.Email);
            // Block if this assessment does not belong to this instructor's session
            if (instructor == null || assessment.Enrollment?.Session?.InstructorId != instructor.InstructorId)
                return Forbid();
        }

        var vm = new AssessmentEditViewModel
        {
            AssessmentId = assessment.AssessmentId,
            EnrollmentId = assessment.EnrollmentId,
            InstructorId = assessment.InstructorId,
            Score        = assessment.Score,
            Result       = assessment.Result
        };

        await LoadDropdownsAsync(vm);
        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "TrainingCoordinator,Instructor")]
    public async Task<IActionResult> Edit(int id, AssessmentEditViewModel vm)
    {
        if (id != vm.AssessmentId) return NotFound();

        if (vm.Result.HasValue && vm.Result != 0 && vm.Result != 1)
            ModelState.AddModelError(string.Empty, "Result must be 0 (Fail) or 1 (Pass).");

        if (User.IsInRole("Instructor") && !User.IsInRole("TrainingCoordinator"))
        {
            var scopeError = await ValidateInstructorScopeAsync(vm.EnrollmentId, vm.InstructorId);
            if (scopeError != null)
                ModelState.AddModelError(string.Empty, scopeError);
        }

        if (ModelState.IsValid)
        {
            var assessment = new Assessment
            {
                AssessmentId = vm.AssessmentId,
                EnrollmentId = vm.EnrollmentId,
                InstructorId = vm.InstructorId,
                Score        = vm.Score,
                Result       = vm.Result
            };

            try
            {
                _context.Update(assessment);
                await _context.SaveChangesAsync();
                await _progressService.RecalculateFromEnrollmentAsync(assessment.EnrollmentId);
                TempData["Success"] = "Assessment updated. Certification progress recalculated.";
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await AssessmentExists(vm.AssessmentId)) return NotFound();
                throw;
            }
            return RedirectToAction(nameof(Index));
        }

        await LoadDropdownsAsync(vm);
        return View(vm);
    }


    [Authorize(Roles = "TrainingCoordinator,Instructor")]
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null) return NotFound();

        var a = await _context.Assessments
            .Include(a => a.Enrollment)
                .ThenInclude(e => e.Trainee)
            .Include(a => a.Enrollment)
                .ThenInclude(e => e.Session)
                    .ThenInclude(s => s.Course)
            .Include(a => a.Instructor)
            .AsNoTracking()
            .FirstOrDefaultAsync(a => a.AssessmentId == id);

        if (a == null) return NotFound();

        if (User.IsInRole("Instructor") && !User.IsInRole("TrainingCoordinator"))
        {
            var user = await _userManager.GetUserAsync(User);
            var instructor = await _context.Instructors.AsNoTracking()
                .FirstOrDefaultAsync(i => i.Email == user!.Email);
            if (instructor == null || a.Enrollment?.Session?.InstructorId != instructor.InstructorId)
                return Forbid();
        }

        var vm = new AssessmentDeleteViewModel
        {
            AssessmentId   = a.AssessmentId,
            TraineeName    = a.Enrollment?.Trainee?.FullName,
            CourseName     = a.Enrollment?.Session?.Course?.CourseName,
            InstructorName = a.Instructor?.FullName,
            Score          = a.Score,
            Result         = a.Result
        };

        return View(vm);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "TrainingCoordinator,Instructor")]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var assessment = await _context.Assessments
            .Include(a => a.Enrollment)
                .ThenInclude(e => e.Session)
            .FirstOrDefaultAsync(a => a.AssessmentId == id);

        if (assessment != null)
        {
            if (User.IsInRole("Instructor") && !User.IsInRole("TrainingCoordinator"))
            {
                var user = await _userManager.GetUserAsync(User);
                var instructor = await _context.Instructors.AsNoTracking()
                    .FirstOrDefaultAsync(i => i.Email == user!.Email);
                if (instructor == null || assessment.Enrollment?.Session?.InstructorId != instructor.InstructorId)
                    return Forbid();
            }

            int enrollmentId = assessment.EnrollmentId;
            _context.Assessments.Remove(assessment);
            await _context.SaveChangesAsync();
            await _progressService.RecalculateFromEnrollmentAsync(enrollmentId);
            TempData["Success"] = "Assessment deleted. Certification progress recalculated.";
        }
        return RedirectToAction(nameof(Index));
    }

    private async Task<string?> ValidateInstructorScopeAsync(int enrollmentId, int? postedInstructorId)
    {
        var user = await _userManager.GetUserAsync(User);
        var instructor = await _context.Instructors.AsNoTracking()
            .FirstOrDefaultAsync(i => i.Email == user!.Email);
        if (instructor == null)
            return "Your instructor profile was not found.";

        // Prevent posting another instructor's ID
        if (postedInstructorId.HasValue && postedInstructorId != instructor.InstructorId)
            return "You can only record assessments under your own instructor profile.";

        var enrollment = await _context.Enrollments
            .Include(e => e.Session)
            .AsNoTracking()
            .FirstOrDefaultAsync(e => e.EnrollmentId == enrollmentId);

        if (enrollment == null)
            return "Enrollment not found.";

        if (enrollment.Session.InstructorId != instructor.InstructorId)
            return "You can only record assessments for sessions assigned to you.";

        return null;
    }


    private async Task LoadDropdownsAsync(AssessmentCreateViewModel vm)
    {
        if (User.IsInRole("Instructor") && !User.IsInRole("TrainingCoordinator"))
        {
            var user = await _userManager.GetUserAsync(User);
            var instructor = await _context.Instructors.AsNoTracking()
                .FirstOrDefaultAsync(i => i.Email == user!.Email);

            if (instructor != null)
            {
                var enrollments = await _context.Enrollments
                    .Include(e => e.Trainee)
                    .Include(e => e.Session)
                        .ThenInclude(s => s.Course)
                    .Where(e => e.Session.InstructorId == instructor.InstructorId)
                    .AsNoTracking()
                    .OrderBy(e => e.Trainee.FullName)
                    .ToListAsync();

                vm.Enrollments = enrollments.Select(e => new SelectListItem
                {
                    Value = e.EnrollmentId.ToString(),
                    Text  = e.Trainee.FullName + " — " + e.Session.Course.CourseName
                }).ToList();

                vm.Instructors = new List<SelectListItem>
                {
                    new SelectListItem
                    {
                        Value    = instructor.InstructorId.ToString(),
                        Text     = instructor.FullName,
                        Selected = true
                    }
                };
            }
        }
        else
        {
            var enrollments = await _context.Enrollments
                .Include(e => e.Trainee)
                .Include(e => e.Session)
                    .ThenInclude(s => s.Course)
                .AsNoTracking()
                .OrderBy(e => e.Trainee.FullName)
                .ToListAsync();

            vm.Enrollments = enrollments.Select(e => new SelectListItem
            {
                Value = e.EnrollmentId.ToString(),
                Text  = e.Trainee.FullName + " — " + e.Session.Course.CourseName
            }).ToList();

            vm.Instructors = await _context.Instructors.AsNoTracking()
                .OrderBy(i => i.FullName)
                .Select(i => new SelectListItem { Value = i.InstructorId.ToString(), Text = i.FullName })
                .ToListAsync();
        }
    }

    private async Task LoadDropdownsAsync(AssessmentEditViewModel vm)
    {
        if (User.IsInRole("Instructor") && !User.IsInRole("TrainingCoordinator"))
        {
            var user = await _userManager.GetUserAsync(User);
            var instructor = await _context.Instructors.AsNoTracking()
                .FirstOrDefaultAsync(i => i.Email == user!.Email);

            if (instructor != null)
            {
                var enrollments = await _context.Enrollments
                    .Include(e => e.Trainee)
                    .Include(e => e.Session)
                        .ThenInclude(s => s.Course)
                    .Where(e => e.Session.InstructorId == instructor.InstructorId)
                    .AsNoTracking()
                    .OrderBy(e => e.Trainee.FullName)
                    .ToListAsync();

                vm.Enrollments = enrollments.Select(e => new SelectListItem
                {
                    Value = e.EnrollmentId.ToString(),
                    Text  = e.Trainee.FullName + " — " + e.Session.Course.CourseName
                }).ToList();

                vm.Instructors = new List<SelectListItem>
                {
                    new SelectListItem
                    {
                        Value    = instructor.InstructorId.ToString(),
                        Text     = instructor.FullName,
                        Selected = true
                    }
                };
            }
        }
        else
        {
            var enrollments = await _context.Enrollments
                .Include(e => e.Trainee)
                .Include(e => e.Session)
                    .ThenInclude(s => s.Course)
                .AsNoTracking()
                .OrderBy(e => e.Trainee.FullName)
                .ToListAsync();

            vm.Enrollments = enrollments.Select(e => new SelectListItem
            {
                Value = e.EnrollmentId.ToString(),
                Text  = e.Trainee.FullName + " — " + e.Session.Course.CourseName
            }).ToList();

            vm.Instructors = await _context.Instructors.AsNoTracking()
                .OrderBy(i => i.FullName)
                .Select(i => new SelectListItem { Value = i.InstructorId.ToString(), Text = i.FullName })
                .ToListAsync();
        }
    }

    private async Task<bool> AssessmentExists(int id) =>
        await _context.Assessments.AnyAsync(a => a.AssessmentId == id);
}
