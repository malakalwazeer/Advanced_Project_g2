using CourseManagementAPI.Data;
using CourseManagementAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace CourseManagement.Controllers;

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

    // GET: TraineeCertificationProgress/Details/5/3
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

        // Attach computed counts for the view
        ViewData["RequiredCourseCount"] = await CountRequiredCourses(certificationId.Value);
        ViewData["PassedCourseCount"] = await CountPassedRequiredCourses(traineeId.Value, certificationId.Value);

        return View(progress);
    }

    // GET: TraineeCertificationProgress/Create
    public IActionResult Create()
    {
        LoadDropdowns();
        return View();
    }

    // POST: TraineeCertificationProgress/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(
        [Bind("TraineeId,CertificationId")]
        TraineeCertificationProgress progress)
    {
        // Prevent duplicate composite key
        var exists = await _context.TraineeCertificationProgresses
            .AnyAsync(p => p.TraineeId == progress.TraineeId &&
                           p.CertificationId == progress.CertificationId);

        if (exists)
        {
            ModelState.AddModelError(string.Empty,
                "A progress record already exists for this trainee and certification.");
        }

        if (ModelState.IsValid)
        {
            await RecalculateProgress(progress);

            _context.Add(progress);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        LoadDropdowns(progress.TraineeId, progress.CertificationId);
        return View(progress);
    }

    // GET: TraineeCertificationProgress/Edit/5/3
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

    // POST: TraineeCertificationProgress/Edit/5/3
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int traineeId, int certificationId,
        [Bind("TraineeId,CertificationId,AchievedDate,ProgressPercentage")]
        TraineeCertificationProgress progress)
    {
        if (traineeId != progress.TraineeId || certificationId != progress.CertificationId)
            return NotFound();

        if (ModelState.IsValid)
        {
            // Recalculate and override any manually entered percentage with computed value
            await RecalculateProgress(progress);

            try
            {
                _context.Update(progress);
                await _context.SaveChangesAsync();
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

    // GET: TraineeCertificationProgress/Delete/5/3
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

    // POST: TraineeCertificationProgress/Delete/5/3
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
        }

        return RedirectToAction(nameof(Index));
    }

    // ─── Business Rule Helpers ────────────────────────────────────────────────

    /// <summary>
    /// Calculates ProgressPercentage (required passed / total required * 100)
    /// and sets AchievedDate when progress reaches 100%.
    /// Modifies the passed-in progress object in place.
    /// </summary>
    private async Task RecalculateProgress(TraineeCertificationProgress progress)
    {
        var totalRequired = await CountRequiredCourses(progress.CertificationId);
        var passed = totalRequired > 0
            ? await CountPassedRequiredCourses(progress.TraineeId, progress.CertificationId)
            : 0;

        progress.ProgressPercentage = totalRequired > 0
            ? Math.Round((decimal)passed / totalRequired * 100, 2)
            : 0m;

        if (progress.ProgressPercentage >= 100)
        {
            progress.AchievedDate ??= DateOnly.FromDateTime(DateTime.Today);
        }
        else
        {
            progress.AchievedDate = null;
        }
    }

    /// <summary>Returns the number of required courses for a certification.</summary>
    private async Task<int> CountRequiredCourses(int certificationId) =>
        await _context.CertificationCourses
            .CountAsync(cc => cc.CertificationId == certificationId && cc.IsRequired);

    /// <summary>Returns how many required courses the trainee has passed.</summary>
    private async Task<int> CountPassedRequiredCourses(int traineeId, int certificationId)
    {
        var requiredCourseIds = await _context.CertificationCourses
            .Where(cc => cc.CertificationId == certificationId && cc.IsRequired)
            .Select(cc => cc.CourseId)
            .ToListAsync();

        var passedCourseIds = await _context.Assessments
            .Include(a => a.Enrollment)
                .ThenInclude(e => e.Session)
            .Where(a => a.Enrollment.TraineeId == traineeId && a.Result == 1)
            .Select(a => a.Enrollment.Session.CourseId)
            .Distinct()
            .ToListAsync();

        return requiredCourseIds.Count(id => passedCourseIds.Contains(id));
    }

    private void LoadDropdowns(int? selectedTraineeId = null, int? selectedCertificationId = null)
    {
        ViewData["TraineeId"] = new SelectList(
            _context.Trainees.AsNoTracking()
                .Select(t => new { t.TraineeId, t.FullName }),
            "TraineeId", "FullName", selectedTraineeId);

        ViewData["CertificationId"] = new SelectList(
            _context.Certifications.AsNoTracking()
                .Select(c => new { c.CertificationId, c.Name }),
            "CertificationId", "Name", selectedCertificationId);
    }
}
