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

//[Authorize(Roles = "Coordinator,Instructor")]
[Authorize(Roles = "TrainingCoordinator,Instructor")]//malak
public class AssessmentsController : Controller
{
    private readonly CourseManagementDbContext _context;
    private readonly AssessmentValidationService _assessmentValidator;

    public AssessmentsController(
        CourseManagementDbContext context,
        AssessmentValidationService assessmentValidator)
    {
        _context = context;
        _assessmentValidator = assessmentValidator;
    }

    public async Task<IActionResult> Index()
    {
        var assessments = await _context.Assessments
            .Include(a => a.Enrollment)
                .ThenInclude(e => e.Trainee)
            .Include(a => a.Enrollment)
                .ThenInclude(e => e.Session)
                    .ThenInclude(s => s.Course)
            .Include(a => a.Instructor)
            .AsNoTracking()
            .ToListAsync();

        var vm = assessments.Select(a => new AssessmentIndexViewModel
        {
            AssessmentId   = a.AssessmentId,
            TraineeName    = a.Enrollment?.Trainee?.FullName,
            CourseName     = a.Enrollment?.Session?.Course?.CourseName,
            InstructorName = a.Instructor?.FullName,
            Score          = a.Score,
            Result         = a.Result
        }).ToList();

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

    public IActionResult Create()
    {
        var vm = new AssessmentCreateViewModel();
        LoadDropdowns(vm);
        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(AssessmentCreateViewModel vm)
    {
        if (vm.Result.HasValue && vm.Result != 0 && vm.Result != 1)
            ModelState.AddModelError(string.Empty, "Result must be 0 (Fail) or 1 (Pass).");

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
                LoadDropdowns(vm);
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

            await UpdateCertificationProgressAsync(assessment.EnrollmentId);

            TempData["Success"] = "Assessment saved. Certification progress updated.";
            return RedirectToAction(nameof(Index));
        }

        LoadDropdowns(vm);
        return View(vm);
    }

    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null) return NotFound();

        var assessment = await _context.Assessments.AsNoTracking()
            .FirstOrDefaultAsync(a => a.AssessmentId == id);
        if (assessment == null) return NotFound();

        var vm = new AssessmentEditViewModel
        {
            AssessmentId = assessment.AssessmentId,
            EnrollmentId = assessment.EnrollmentId,
            InstructorId = assessment.InstructorId,
            Score        = assessment.Score,
            Result       = assessment.Result
        };

        LoadDropdowns(vm);
        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, AssessmentEditViewModel vm)
    {
        if (id != vm.AssessmentId) return NotFound();

        if (vm.Result.HasValue && vm.Result != 0 && vm.Result != 1)
            ModelState.AddModelError(string.Empty, "Result must be 0 (Fail) or 1 (Pass).");

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

                await UpdateCertificationProgressAsync(assessment.EnrollmentId);

                TempData["Success"] = "Assessment updated. Certification progress recalculated.";
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await AssessmentExists(vm.AssessmentId)) return NotFound();
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
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var assessment = await _context.Assessments.FindAsync(id);
        if (assessment != null)
        {
            int enrollmentId = assessment.EnrollmentId;

            _context.Assessments.Remove(assessment);
            await _context.SaveChangesAsync();

            await UpdateCertificationProgressAsync(enrollmentId);

            TempData["Success"] = "Assessment deleted. Certification progress recalculated.";
        }
        return RedirectToAction(nameof(Index));
    }

    // ─── Helpers ─────────────────────────────────────────────────────────────

    private async Task UpdateCertificationProgressAsync(int enrollmentId)
    {
        var enrollment = await _context.Enrollments
            .Include(e => e.Session)
            .AsNoTracking()
            .FirstOrDefaultAsync(e => e.EnrollmentId == enrollmentId);

        if (enrollment == null) return;

        int traineeId = enrollment.TraineeId;

        var traineePassedCourseIds = await _context.Assessments
            .Include(a => a.Enrollment)
                .ThenInclude(e => e.Session)
            .Where(a => a.Enrollment.TraineeId == traineeId && a.Result == 1)
            .Select(a => a.Enrollment.Session.CourseId)
            .Distinct()
            .ToListAsync();

        var relevantCertIds = await _context.CertificationCourses
            .Where(cc => traineePassedCourseIds.Contains(cc.CourseId))
            .Select(cc => cc.CertificationId)
            .Distinct()
            .ToListAsync();

        foreach (var certId in relevantCertIds)
        {
            var requiredIds = await _context.CertificationCourses
                .Where(cc => cc.CertificationId == certId && cc.IsRequired)
                .Select(cc => cc.CourseId)
                .ToListAsync();

            if (requiredIds.Count == 0) continue;

            var passedCount = requiredIds.Count(c => traineePassedCourseIds.Contains(c));
            var pct = Math.Round((decimal)passedCount / requiredIds.Count * 100, 2);

            var progress = await _context.TraineeCertificationProgresses
                .FirstOrDefaultAsync(p => p.TraineeId == traineeId && p.CertificationId == certId);

            if (progress == null)
            {
                _context.TraineeCertificationProgresses.Add(new TraineeCertificationProgress
                {
                    TraineeId          = traineeId,
                    CertificationId    = certId,
                    ProgressPercentage = pct,
                    AchievedDate       = pct >= 100 ? DateOnly.FromDateTime(DateTime.Today) : null
                });
            }
            else
            {
                progress.ProgressPercentage = pct;
                if (pct >= 100 && progress.AchievedDate == null)
                    progress.AchievedDate = DateOnly.FromDateTime(DateTime.Today);
                else if (pct < 100)
                    progress.AchievedDate = null;
            }
        }

        await _context.SaveChangesAsync();
    }

    private void LoadDropdowns(AssessmentCreateViewModel vm)
    {
        vm.Enrollments = _context.Enrollments
            .Include(e => e.Trainee)
            .Include(e => e.Session)
                .ThenInclude(s => s.Course)
            .AsNoTracking()
            .OrderBy(e => e.Trainee.FullName)
            .AsEnumerable()
            .Select(e => new SelectListItem
            {
                Value = e.EnrollmentId.ToString(),
                Text  = e.Trainee.FullName + " — " + e.Session.Course.CourseName
            }).ToList();

        vm.Instructors = _context.Instructors.AsNoTracking()
            .OrderBy(i => i.FullName)
            .Select(i => new SelectListItem { Value = i.InstructorId.ToString(), Text = i.FullName })
            .ToList();
    }

    private void LoadDropdowns(AssessmentEditViewModel vm)
    {
        vm.Enrollments = _context.Enrollments
            .Include(e => e.Trainee)
            .Include(e => e.Session)
                .ThenInclude(s => s.Course)
            .AsNoTracking()
            .OrderBy(e => e.Trainee.FullName)
            .AsEnumerable()
            .Select(e => new SelectListItem
            {
                Value = e.EnrollmentId.ToString(),
                Text  = e.Trainee.FullName + " — " + e.Session.Course.CourseName
            }).ToList();

        vm.Instructors = _context.Instructors.AsNoTracking()
            .OrderBy(i => i.FullName)
            .Select(i => new SelectListItem { Value = i.InstructorId.ToString(), Text = i.FullName })
            .ToList();
    }

    private async Task<bool> AssessmentExists(int id) =>
        await _context.Assessments.AnyAsync(a => a.AssessmentId == id);
}
