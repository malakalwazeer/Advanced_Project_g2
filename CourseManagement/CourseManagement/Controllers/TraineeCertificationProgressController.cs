using CourseManagement.Services;
using CourseManagement.ViewModels;
using CourseManagementAPI.Data;
using CourseManagementAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CourseManagement.Controllers;

[Authorize(Roles = "TrainingCoordinator,Instructor,Trainee")]
public class TraineeCertificationProgressController : Controller
{
    private readonly CourseManagementDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ICertificateService _certificateService;
    private readonly CertificationProgressService _progressService;

    public TraineeCertificationProgressController(
        CourseManagementDbContext context,
        UserManager<ApplicationUser> userManager,
        ICertificateService certificateService,
        CertificationProgressService progressService)
    {
        _context = context;
        _userManager = userManager;
        _certificateService = certificateService;
        _progressService = progressService;
    }

    public async Task<IActionResult> Index(string? searchString, bool? achievedOnly)
    {
        var user = await _userManager.GetUserAsync(User);

        if (User.IsInRole("Trainee"))
        {
            var trainee = await _context.Trainees.AsNoTracking()
                .FirstOrDefaultAsync(t => t.Email == user!.Email);
            if (trainee == null) return Forbid();

            // Refresh progress for this trainee before displaying
            await _progressService.RecalculateForTraineeAsync(trainee.TraineeId);
        }

        var query = _context.TraineeCertificationProgresses
            .Include(p => p.Trainee)
            .Include(p => p.Certification)
            .AsNoTracking()
            .AsQueryable();

        if (User.IsInRole("Trainee"))
        {
            var trainee = await _context.Trainees.AsNoTracking()
                .FirstOrDefaultAsync(t => t.Email == user!.Email);
            query = query.Where(p => p.TraineeId == trainee!.TraineeId);
        }
        else if (User.IsInRole("Instructor"))
        {
            var instructor = await _context.Instructors.AsNoTracking()
                .FirstOrDefaultAsync(i => i.Email == user!.Email);
            if (instructor == null) return Forbid();

            var traineeIds = await _context.Enrollments
                .Include(e => e.Session)
                .Where(e => e.Session.InstructorId == instructor.InstructorId)
                .Select(e => e.TraineeId)
                .Distinct()
                .ToListAsync();

            query = query.Where(p => traineeIds.Contains(p.TraineeId));
        }

        if (!string.IsNullOrWhiteSpace(searchString))
        {
            var term = searchString.ToLower();
            query = query.Where(p =>
                p.Trainee.FullName.ToLower().Contains(term) ||
                p.Certification.Name.ToLower().Contains(term));
        }

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

        var user = await _userManager.GetUserAsync(User);

        if (User.IsInRole("Trainee"))
        {
            var ownTrainee = await _context.Trainees.AsNoTracking()
                .FirstOrDefaultAsync(t => t.Email == user!.Email);
            if (ownTrainee == null || ownTrainee.TraineeId != traineeId)
                return Forbid();
        }
        else if (User.IsInRole("Instructor"))
        {
            var instructor = await _context.Instructors.AsNoTracking()
                .FirstOrDefaultAsync(i => i.Email == user!.Email);
            if (instructor == null) return Forbid();

            var isRelated = await _context.Enrollments
                .Include(e => e.Session)
                .AnyAsync(e => e.Session.InstructorId == instructor.InstructorId
                            && e.TraineeId == traineeId);
            if (!isRelated) return Forbid();
        }

        // Refresh progress for this trainee before displaying details
        await _progressService.RecalculateForTraineeAsync(traineeId.Value);

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
            TraineeId            = p.TraineeId,
            CertificationId      = p.CertificationId,
            TraineeName          = p.Trainee?.FullName,
            CertificationName    = p.Certification?.Name,
            ProgressPercentage   = p.ProgressPercentage,
            AchievedDate         = p.AchievedDate,
            RequiredCoursesCount = requiredCount,
            PassedCoursesCount   = passedCount,
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
}
