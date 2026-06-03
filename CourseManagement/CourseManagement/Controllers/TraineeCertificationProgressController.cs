using CourseManagement.Services;
using CourseManagement.ViewModels;
using CourseManagementAPI.Data;
using CourseManagementAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace CourseManagement.Controllers;

[Authorize(Roles = "TrainingCoordinator,Trainee")]
public class TraineeCertificationProgressController : Controller
{
    private readonly CourseManagementDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ICertificateService _certificateService;

    public TraineeCertificationProgressController(
        CourseManagementDbContext context,
        UserManager<ApplicationUser> userManager,
        ICertificateService certificateService)
    {
        _context = context;
        _userManager = userManager;
        _certificateService = certificateService;
    }

    public async Task<IActionResult> Index(string? searchString, bool? achievedOnly)
    {
        var query = _context.TraineeCertificationProgresses
            .Include(p => p.Trainee)
            .Include(p => p.Certification)
            .AsNoTracking()
            .AsQueryable();

        // Role-based restriction applied first so search cannot bypass it
        if (User.IsInRole("Trainee"))
        {
            var user = await _userManager.GetUserAsync(User);
            var trainee = await _context.Trainees.AsNoTracking()
                .FirstOrDefaultAsync(t => t.Email == user!.Email);
            if (trainee == null) return Forbid();
            query = query.Where(p => p.TraineeId == trainee.TraineeId);
        }

        if (!string.IsNullOrWhiteSpace(searchString))
        {
            var term = searchString.ToLower();
            query = query.Where(p =>
                p.Trainee.FullName.ToLower().Contains(term) ||
                p.Certification.Name.ToLower().Contains(term));
        }

        // Achieved = certification fully completed (AchievedDate is set)
        if (achievedOnly == true)
            query = query.Where(p => p.AchievedDate != null);

        var progresses = await query.ToListAsync();

        var vm = progresses.Select(p => new TraineeCertificationProgressIndexViewModel
        {
            TraineeId          = p.TraineeId,
            CertificationId    = p.CertificationId,
            TraineeName        = p.Trainee?.FullName,
            CertificationName  = p.Certification?.Name,
            ProgressPercentage = p.ProgressPercentage,
            AchievedDate       = p.AchievedDate
        }).ToList();

        ViewBag.SearchString = searchString;
        ViewBag.AchievedOnly = achievedOnly;

        return View(vm);
    }

    public async Task<IActionResult> Details(int? traineeId, int? certificationId)
    {
        if (traineeId == null || certificationId == null) return NotFound();

        if (User.IsInRole("Trainee"))
        {
            var user = await _userManager.GetUserAsync(User);
            var ownTrainee = await _context.Trainees.AsNoTracking()
                .FirstOrDefaultAsync(t => t.Email == user!.Email);
            if (ownTrainee == null || ownTrainee.TraineeId != traineeId)
                return Forbid();
        }

        var p = await _context.TraineeCertificationProgresses
            .Include(p => p.Trainee)
            .Include(p => p.Certification)
                .ThenInclude(c => c.CertificationCourses)
                    .ThenInclude(cc => cc.Course)
            .AsNoTracking()
            .FirstOrDefaultAsync(p =>
                p.TraineeId == traineeId && p.CertificationId == certificationId);

        if (p == null) return NotFound();

        int requiredCount = await CountRequiredCourses(certificationId.Value);
        int passedCount   = await CountPassedRequiredCourses(traineeId.Value, certificationId.Value);

        var vm = new TraineeCertificationProgressDetailsViewModel
        {
            TraineeId           = p.TraineeId,
            CertificationId     = p.CertificationId,
            TraineeName         = p.Trainee?.FullName,
            CertificationName   = p.Certification?.Name,
            ProgressPercentage  = p.ProgressPercentage,
            AchievedDate        = p.AchievedDate,
            RequiredCoursesCount = requiredCount,
            PassedCoursesCount  = passedCount,
            CertificationCourses = (p.Certification?.CertificationCourses ?? [])
                .Select(cc => new CertCourseRow
                {
                    CourseName = cc.Course?.CourseName,
                    IsRequired = cc.IsRequired
                }).ToList()
        };

        return View(vm);
    }

    [Authorize(Roles = "TrainingCoordinator,Trainee")]
    public async Task<IActionResult> Download(int? traineeId, int? certificationId)
    {
        if (traineeId == null || certificationId == null) return NotFound();

        var p = await _context.TraineeCertificationProgresses
            .Include(p => p.Trainee)
            .Include(p => p.Certification)
            .AsNoTracking()
            .FirstOrDefaultAsync(p =>
                p.TraineeId == traineeId && p.CertificationId == certificationId);

        if (p == null || p.AchievedDate == null) return NotFound();

        byte[] pdfData = _certificateService.GenerateCertificate(
            p.Trainee?.FullName ?? "Unknown Trainee",
            p.Certification?.Name ?? "Unknown Certification",
            p.AchievedDate);
            
        return File(pdfData, "application/pdf", $"Certificate_{p.Trainee?.FullName.Replace(" ", "_")}.pdf");
    }

    [Authorize(Roles = "TrainingCoordinator")]
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

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "TrainingCoordinator")]
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

    [Authorize(Roles = "TrainingCoordinator")]
    public async Task<IActionResult> Delete(int? traineeId, int? certificationId)
    {
        if (traineeId == null || certificationId == null) return NotFound();

        var p = await _context.TraineeCertificationProgresses
            .Include(p => p.Trainee)
            .Include(p => p.Certification)
            .AsNoTracking()
            .FirstOrDefaultAsync(p =>
                p.TraineeId == traineeId && p.CertificationId == certificationId);

        if (p == null) return NotFound();

        var vm = new TraineeCertificationProgressDeleteViewModel
        {
            TraineeId         = p.TraineeId,
            CertificationId   = p.CertificationId,
            TraineeName       = p.Trainee?.FullName,
            CertificationName = p.Certification?.Name,
            ProgressPercentage = p.ProgressPercentage,
            AchievedDate      = p.AchievedDate
        };

        return View(vm);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "TrainingCoordinator")]
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

    // ─── Helpers ─────────────────────────────────────────────────────────────

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
