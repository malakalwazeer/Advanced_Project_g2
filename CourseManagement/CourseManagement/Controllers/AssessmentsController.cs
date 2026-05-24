using CourseManagementAPI.Data;
using CourseManagementAPI.Dtos;
using CourseManagementAPI.Models;
using CourseManagementAPI.Services.Validation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace CourseManagement.Controllers;

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

    // GET: Assessments
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

        return View(assessments);
    }

    // GET: Assessments/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null) return NotFound();

        var assessment = await _context.Assessments
            .Include(a => a.Enrollment)
                .ThenInclude(e => e.Trainee)
            .Include(a => a.Enrollment)
                .ThenInclude(e => e.Session)
                    .ThenInclude(s => s.Course)
            .Include(a => a.Instructor)
            .AsNoTracking()
            .FirstOrDefaultAsync(a => a.AssessmentId == id);

        if (assessment == null) return NotFound();

        return View(assessment);
    }

    // GET: Assessments/Create
    public IActionResult Create()
    {
        LoadDropdowns();
        return View();
    }

    // POST: Assessments/Create
    // Uses AssessmentValidationService (from API project, injected via DI) which checks:
    //   • Enrollment exists
    //   • Instructor exists and is assigned to the session
    //   • Session has already ended (EndDateTime <= now)
    //   • No duplicate assessment for this enrollment
    // After passing validation, saves via EF Core and triggers certification progress update.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(
        [Bind("EnrollmentId,InstructorId,Score,Result")]
        Assessment assessment)
    {
        ModelState.Remove(nameof(Assessment.Enrollment));
        ModelState.Remove(nameof(Assessment.Instructor));

        // Score/result range check before calling the service.
        if (!ValidateScoreAndResult(assessment, out var rangeError))
            ModelState.AddModelError(string.Empty, rangeError!);

        if (ModelState.IsValid)
        {
            // Map to the DTO the service expects.
            var dto = new CreateAssessmentDto
            {
                EnrollmentId = assessment.EnrollmentId,
                InstructorId = assessment.InstructorId,
                Result       = assessment.Result ?? 0,
                Score        = assessment.Score
            };

            var error = await _assessmentValidator.ValidateCreateAsync(dto);
            if (error != null)
            {
                ModelState.AddModelError(string.Empty, error);
                LoadDropdowns(assessment.EnrollmentId, assessment.InstructorId);
                return View(assessment);
            }

            _context.Add(assessment);
            await _context.SaveChangesAsync();

            await UpdateCertificationProgressAsync(assessment.EnrollmentId);

            TempData["Success"] = "Assessment saved. Certification progress updated.";
            return RedirectToAction(nameof(Index));
        }

        LoadDropdowns(assessment.EnrollmentId, assessment.InstructorId);
        return View(assessment);
    }

    // GET: Assessments/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null) return NotFound();

        var assessment = await _context.Assessments.FindAsync(id);
        if (assessment == null) return NotFound();

        LoadDropdowns(assessment.EnrollmentId, assessment.InstructorId);
        return View(assessment);
    }

    // POST: Assessments/Edit/5
    // Edit does NOT use AssessmentValidationService — the service's "assessment already exists"
    // check would always fail for an existing record being edited. Score/result range
    // validation is applied locally instead.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id,
        [Bind("AssessmentId,EnrollmentId,InstructorId,Score,Result")]
        Assessment assessment)
    {
        if (id != assessment.AssessmentId) return NotFound();

        ModelState.Remove(nameof(Assessment.Enrollment));
        ModelState.Remove(nameof(Assessment.Instructor));

        if (!ValidateScoreAndResult(assessment, out var rangeError))
            ModelState.AddModelError(string.Empty, rangeError!);

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(assessment);
                await _context.SaveChangesAsync();

                await UpdateCertificationProgressAsync(assessment.EnrollmentId);

                TempData["Success"] = "Assessment updated. Certification progress recalculated.";
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await AssessmentExists(assessment.AssessmentId))
                    return NotFound();
                throw;
            }
            return RedirectToAction(nameof(Index));
        }

        LoadDropdowns(assessment.EnrollmentId, assessment.InstructorId);
        return View(assessment);
    }

    // GET: Assessments/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null) return NotFound();

        var assessment = await _context.Assessments
            .Include(a => a.Enrollment)
                .ThenInclude(e => e.Trainee)
            .Include(a => a.Enrollment)
                .ThenInclude(e => e.Session)
                    .ThenInclude(s => s.Course)
            .Include(a => a.Instructor)
            .AsNoTracking()
            .FirstOrDefaultAsync(a => a.AssessmentId == id);

        if (assessment == null) return NotFound();

        return View(assessment);
    }

    // POST: Assessments/Delete/5
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

    /// <summary>Validates Score (0–100) and Result (0 or 1). Used for both Create and Edit.</summary>
    private static bool ValidateScoreAndResult(Assessment assessment, out string? errorMessage)
    {
        if (assessment.Score.HasValue && (assessment.Score < 0 || assessment.Score > 100))
        {
            errorMessage = "Score must be between 0 and 100.";
            return false;
        }

        if (assessment.Result.HasValue && assessment.Result != 0 && assessment.Result != 1)
        {
            errorMessage = "Result must be 0 (Fail) or 1 (Pass).";
            return false;
        }

        errorMessage = null;
        return true;
    }

    /// <summary>
    /// Recalculates TraineeCertificationProgress for all certifications whose required courses
    /// include any course the trainee has passed. Called after every Create/Edit/Delete.
    /// </summary>
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
                    TraineeId           = traineeId,
                    CertificationId     = certId,
                    ProgressPercentage  = pct,
                    AchievedDate        = pct >= 100 ? DateOnly.FromDateTime(DateTime.Today) : null
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

    private void LoadDropdowns(int? selectedEnrollmentId = null, int? selectedInstructorId = null)
    {
        ViewData["EnrollmentId"] = new SelectList(
            _context.Enrollments
                .Include(e => e.Trainee)
                .Include(e => e.Session)
                    .ThenInclude(s => s.Course)
                .AsNoTracking()
                .OrderBy(e => e.Trainee.FullName)
                .Select(e => new
                {
                    e.EnrollmentId,
                    Display = e.Trainee.FullName + " — " + e.Session.Course.CourseName
                }),
            "EnrollmentId", "Display", selectedEnrollmentId);

        ViewData["InstructorId"] = new SelectList(
            _context.Instructors.AsNoTracking()
                .OrderBy(i => i.FullName)
                .Select(i => new { i.InstructorId, i.FullName }),
            "InstructorId", "FullName", selectedInstructorId);
    }

    private async Task<bool> AssessmentExists(int id) =>
        await _context.Assessments.AnyAsync(a => a.AssessmentId == id);
}
