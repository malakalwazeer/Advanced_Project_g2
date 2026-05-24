using CourseManagementAPI.Data;
using CourseManagementAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace CourseManagement.Controllers;

[Authorize(Roles = "TrainingCoordinator,Trainee")]
public class TraineeCertificationProgressController : Controller
{
    private readonly CourseManagementDbContext _context;

    public TraineeCertificationProgressController(CourseManagementDbContext context)
    {
        _context = context;
    }

    // GET: TraineeCertificationProgress
    public async Task<IActionResult> Index()
    {
        var progresses = await _context.TraineeCertificationProgresses
            .Include(p => p.Trainee)
            .Include(p => p.Certification)
            .AsNoTracking()
            .ToListAsync();

        return View(progresses);
    }

    // GET: TraineeCertificationProgress/Details?traineeId=5&certificationId=3
    public async Task<IActionResult> Details(int? traineeId, int? certificationId)
    {
        if (traineeId == null || certificationId == null) return NotFound();

        var progress = await _context.TraineeCertificationProgresses
            .Include(p => p.Trainee)
            .Include(p => p.Certification)
                .ThenInclude(c => c.CertificationCourses)
                    .ThenInclude(cc => cc.Course)
            .AsNoTracking()
            .FirstOrDefaultAsync(p =>
                p.TraineeId == traineeId && p.CertificationId == certificationId);

        if (progress == null) return NotFound();

        ViewData["RequiredCourseCount"] = await CountRequiredCourses(certificationId.Value);
        ViewData["PassedCourseCount"]   = await CountPassedRequiredCourses(traineeId.Value, certificationId.Value);

        return View(progress);
    }

    // GET: TraineeCertificationProgress/Edit?traineeId=5&certificationId=3
    public async Task<IActionResult> Edit(int? traineeId, int? certificationId)
    {
        if (traineeId == null || certificationId == null) return NotFound();

        var progress = await _context.TraineeCertificationProgresses
            .FirstOrDefaultAsync(p =>
                p.TraineeId == traineeId && p.CertificationId == certificationId);

        if (progress == null) return NotFound();

        LoadDropdowns(progress.TraineeId, progress.CertificationId);
        return View(progress);
    }

    // POST: TraineeCertificationProgress/Edit?traineeId=5&certificationId=3
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int traineeId, int certificationId,
        [Bind("TraineeId,CertificationId,AchievedDate,ProgressPercentage")]
        TraineeCertificationProgress progress)
    {
        if (traineeId != progress.TraineeId || certificationId != progress.CertificationId)
            return NotFound();

        ModelState.Remove(nameof(TraineeCertificationProgress.Trainee));
        ModelState.Remove(nameof(TraineeCertificationProgress.Certification));

        if (ModelState.IsValid)
        {
            await RecalculateProgress(progress);

            try
            {
                _context.Update(progress);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Certification progress updated.";
            }
            catch (DbUpdateConcurrencyException)
            {
                var exists = await _context.TraineeCertificationProgresses
                    .AnyAsync(p => p.TraineeId == traineeId && p.CertificationId == certificationId);
                if (!exists) return NotFound();
                throw;
            }
            return RedirectToAction(nameof(Index));
        }

        LoadDropdowns(progress.TraineeId, progress.CertificationId);
        return View(progress);
    }

    // GET: TraineeCertificationProgress/Delete?traineeId=5&certificationId=3
    public async Task<IActionResult> Delete(int? traineeId, int? certificationId)
    {
        if (traineeId == null || certificationId == null) return NotFound();

        var progress = await _context.TraineeCertificationProgresses
            .Include(p => p.Trainee)
            .Include(p => p.Certification)
            .AsNoTracking()
            .FirstOrDefaultAsync(p =>
                p.TraineeId == traineeId && p.CertificationId == certificationId);

        if (progress == null) return NotFound();

        return View(progress);
    }

    // POST: TraineeCertificationProgress/Delete
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int traineeId, int certificationId)
    {
        var progress = await _context.TraineeCertificationProgresses
            .FirstOrDefaultAsync(p =>
                p.TraineeId == traineeId && p.CertificationId == certificationId);

        if (progress != null)
        {
            _context.TraineeCertificationProgresses.Remove(progress);
            await _context.SaveChangesAsync();
            TempData["Success"] = "Certification progress record deleted.";
        }

        return RedirectToAction(nameof(Index));
    }

    // ─── Business Rule Helpers ────────────────────────────────────────────────

    private async Task RecalculateProgress(TraineeCertificationProgress progress)
    {
        var total  = await CountRequiredCourses(progress.CertificationId);
        var passed = total > 0 ? await CountPassedRequiredCourses(progress.TraineeId, progress.CertificationId) : 0;

        progress.ProgressPercentage = total > 0
            ? Math.Round((decimal)passed / total * 100, 2)
            : 0m;

        if (progress.ProgressPercentage >= 100)
            progress.AchievedDate ??= DateOnly.FromDateTime(DateTime.Today);
        else
            progress.AchievedDate = null;
    }

    private async Task<int> CountRequiredCourses(int certificationId) =>
        await _context.CertificationCourses
            .CountAsync(cc => cc.CertificationId == certificationId && cc.IsRequired);

    private async Task<int> CountPassedRequiredCourses(int traineeId, int certificationId)
    {
        var requiredIds = await _context.CertificationCourses
            .Where(cc => cc.CertificationId == certificationId && cc.IsRequired)
            .Select(cc => cc.CourseId)
            .ToListAsync();

        var passedIds = await _context.Assessments
            .Include(a => a.Enrollment)
                .ThenInclude(e => e.Session)
            .Where(a => a.Enrollment.TraineeId == traineeId && a.Result == 1)
            .Select(a => a.Enrollment.Session.CourseId)
            .Distinct()
            .ToListAsync();

        return requiredIds.Count(id => passedIds.Contains(id));
    }

    private void LoadDropdowns(int? selectedTraineeId = null, int? selectedCertificationId = null)
    {
        ViewData["TraineeId"] = new SelectList(
            _context.Trainees.AsNoTracking().OrderBy(t => t.FullName)
                .Select(t => new { t.TraineeId, t.FullName }),
            "TraineeId", "FullName", selectedTraineeId);

        ViewData["CertificationId"] = new SelectList(
            _context.Certifications.AsNoTracking().OrderBy(c => c.Name)
                .Select(c => new { c.CertificationId, c.Name }),
            "CertificationId", "Name", selectedCertificationId);
    }
}
