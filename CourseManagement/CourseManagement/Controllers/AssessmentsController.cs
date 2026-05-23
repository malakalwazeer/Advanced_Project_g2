using CourseManagementAPI.Data;
using CourseManagementAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace CourseManagement.Controllers;

public class AssessmentsController : Controller
{
    private readonly CourseManagementDbContext _context;

    public AssessmentsController(CourseManagementDbContext context)
    {
        _context = context;
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
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(
        [Bind("EnrollmentId,InstructorId,Score,Result")]
        Assessment assessment)
    {
        if (!ValidateScoreAndResult(assessment, out var scoreError))
        {
            ModelState.AddModelError(string.Empty, scoreError!);
            LoadDropdowns(assessment.EnrollmentId, assessment.InstructorId);
            return View(assessment);
        }

        if (ModelState.IsValid)
        {
            _context.Add(assessment);
            await _context.SaveChangesAsync();

            await UpdateCertificationProgressAsync(assessment.EnrollmentId);

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
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id,
        [Bind("AssessmentId,EnrollmentId,InstructorId,Score,Result")]
        Assessment assessment)
    {
        if (id != assessment.AssessmentId) return NotFound();

        if (!ValidateScoreAndResult(assessment, out var scoreError))
        {
            ModelState.AddModelError(string.Empty, scoreError!);
            LoadDropdowns(assessment.EnrollmentId, assessment.InstructorId);
            return View(assessment);
        }

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(assessment);
                await _context.SaveChangesAsync();

                await UpdateCertificationProgressAsync(assessment.EnrollmentId);
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
        var assessment = await _context.Assessments
            .AsNoTracking()
            .FirstOrDefaultAsync(a => a.AssessmentId == id);

        int? enrollmentId = assessment?.EnrollmentId;

        if (assessment != null)
        {
            _context.Assessments.Remove(_context.Assessments.Find(id)!);
            await _context.SaveChangesAsync();

            if (enrollmentId.HasValue)
                await UpdateCertificationProgressAsync(enrollmentId.Value);
        }

        return RedirectToAction(nameof(Index));
    }

    // ─── Business Rule Helpers ────────────────────────────────────────────────

    /// <summary>
    /// Validates Score (0–100) and Result (0 or 1).
    /// Returns false and sets errorMessage when invalid.
    /// </summary>
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
    /// After an assessment is saved or deleted, recalculates certification progress
    /// for the trainee enrolled in the given enrollment.
    /// </summary>
    private async Task UpdateCertificationProgressAsync(int enrollmentId)
    {
        var enrollment = await _context.Enrollments
            .Include(e => e.Session)
            .AsNoTracking()
            .FirstOrDefaultAsync(e => e.EnrollmentId == enrollmentId);

        if (enrollment == null) return;

        int traineeId = enrollment.TraineeId;

        // Find all certifications that contain courses the trainee has enrollments in
        var traineePassedCourseIds = await _context.Assessments
            .Include(a => a.Enrollment)
                .ThenInclude(e => e.Session)
            .Where(a => a.Enrollment.TraineeId == traineeId && a.Result == 1)
            .Select(a => a.Enrollment.Session.CourseId)
            .Distinct()
            .ToListAsync();

        // Get all certifications that have at least one of the trainee's courses
        var relevantCertificationIds = await _context.CertificationCourses
            .Where(cc => traineePassedCourseIds.Contains(cc.CourseId))
            .Select(cc => cc.CertificationId)
            .Distinct()
            .ToListAsync();

        foreach (var certId in relevantCertificationIds)
        {
            var requiredCourses = await _context.CertificationCourses
                .Where(cc => cc.CertificationId == certId && cc.IsRequired)
                .Select(cc => cc.CourseId)
                .ToListAsync();

            if (requiredCourses.Count == 0) continue;

            var passedRequiredCount = requiredCourses
                .Count(courseId => traineePassedCourseIds.Contains(courseId));

            var progressPct = Math.Round(
                (decimal)passedRequiredCount / requiredCourses.Count * 100, 2);

            var progress = await _context.TraineeCertificationProgresses
                .FirstOrDefaultAsync(p => p.TraineeId == traineeId && p.CertificationId == certId);

            if (progress == null)
            {
                progress = new TraineeCertificationProgress
                {
                    TraineeId = traineeId,
                    CertificationId = certId,
                    ProgressPercentage = progressPct,
                    AchievedDate = progressPct >= 100 ? DateOnly.FromDateTime(DateTime.Today) : null
                };
                _context.TraineeCertificationProgresses.Add(progress);
            }
            else
            {
                progress.ProgressPercentage = progressPct;
                if (progressPct >= 100 && progress.AchievedDate == null)
                    progress.AchievedDate = DateOnly.FromDateTime(DateTime.Today);
                else if (progressPct < 100)
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
                .Select(e => new
                {
                    e.EnrollmentId,
                    Display = e.Trainee.FullName + " — " + e.Session.Course.CourseName
                }),
            "EnrollmentId", "Display", selectedEnrollmentId);

        ViewData["InstructorId"] = new SelectList(
            _context.Instructors.AsNoTracking()
                .Select(i => new { i.InstructorId, i.FullName }),
            "InstructorId", "FullName", selectedInstructorId);
    }

    private async Task<bool> AssessmentExists(int id) =>
        await _context.Assessments.AnyAsync(a => a.AssessmentId == id);
}
