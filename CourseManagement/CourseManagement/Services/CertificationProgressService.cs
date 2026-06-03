using CourseManagementAPI.Data;
using CourseManagementAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace CourseManagement.Services;
public class CertificationProgressService
{
    private readonly CourseManagementDbContext _context;

    public CertificationProgressService(CourseManagementDbContext context)
    {
        _context = context;
    }
    public async Task RecalculateForTraineeAsync(int traineeId)
    {
        Console.WriteLine($"[CertProgress] ── RecalculateForTraineeAsync(traineeId={traineeId}) ──");

        // Step 1: find every course this trainee has passed (Result == 1)
        var passedAssessments = await _context.Assessments
            .Include(a => a.Enrollment)
                .ThenInclude(e => e.Session)
            .Where(a => a.Enrollment.TraineeId == traineeId)
            .Select(a => new { a.AssessmentId, a.Result, CourseId = a.Enrollment.Session.CourseId })
            .ToListAsync();

        Console.WriteLine($"[CertProgress]   Total assessments for trainee: {passedAssessments.Count}");
        foreach (var a in passedAssessments)
            Console.WriteLine($"[CertProgress]     AssessmentId={a.AssessmentId}  Result={a.Result?.ToString() ?? "null"}  CourseId={a.CourseId}");

        var passedCourseIds = passedAssessments
            .Where(a => a.Result == 1)
            .Select(a => a.CourseId)
            .Distinct()
            .ToList();

        Console.WriteLine($"[CertProgress]   Passed courses (Result==1): [{string.Join(", ", passedCourseIds)}]");

        // Step 2: certifications that contain any passed course
        var certIdsFromPassed = await _context.CertificationCourses
            .Where(cc => passedCourseIds.Contains(cc.CourseId))
            .Select(cc => cc.CertificationId)
            .Distinct()
            .ToListAsync();

        Console.WriteLine($"[CertProgress]   Certs linked to passed courses: [{string.Join(", ", certIdsFromPassed)}]");

        // Step 3: certifications that already have a progress record (may need to go down)
        var certIdsWithRecord = await _context.TraineeCertificationProgresses
            .Where(p => p.TraineeId == traineeId)
            .Select(p => p.CertificationId)
            .ToListAsync();

        Console.WriteLine($"[CertProgress]   Certs with existing records:   [{string.Join(", ", certIdsWithRecord)}]");

        var certIds = certIdsFromPassed.Union(certIdsWithRecord).ToList();
        Console.WriteLine($"[CertProgress]   Certs to recalculate (union):  [{string.Join(", ", certIds)}]");

        if (certIds.Count == 0)
        {
            Console.WriteLine("[CertProgress]   *** No certifications to recalculate — nothing will be written ***");
        }

        foreach (var certId in certIds)
        {
            var linkedCourseIds = await _context.CertificationCourses
                .Where(cc => cc.CertificationId == certId)
                .Select(cc => cc.CourseId)
                .ToListAsync();

            Console.WriteLine($"[CertProgress]   CertId={certId}: linked courses = [{string.Join(", ", linkedCourseIds)}]");

            if (linkedCourseIds.Count == 0)
            {
                Console.WriteLine($"[CertProgress]   CertId={certId}: skipped — no courses linked to this certification");
                continue;
            }

            int passedCount = linkedCourseIds.Count(id => passedCourseIds.Contains(id));
            decimal pct = Math.Round((decimal)passedCount / linkedCourseIds.Count * 100, 2);

            Console.WriteLine($"[CertProgress]   CertId={certId}: passed {passedCount}/{linkedCourseIds.Count} linked → {pct}%");

            var record = await _context.TraineeCertificationProgresses
                .FirstOrDefaultAsync(p => p.TraineeId == traineeId && p.CertificationId == certId);

            if (record == null)
            {
                if (pct == 0)
                {
                    Console.WriteLine($"[CertProgress]   CertId={certId}: pct=0 and no existing record → skipped");
                    continue;
                }
                _context.TraineeCertificationProgresses.Add(new TraineeCertificationProgress
                {
                    TraineeId          = traineeId,
                    CertificationId    = certId,
                    ProgressPercentage = pct,
                    AchievedDate       = pct >= 100 ? DateOnly.FromDateTime(DateTime.Today) : null
                });
                Console.WriteLine($"[CertProgress]   CertId={certId}: *** CREATED new record at {pct}% ***");
            }
            else
            {
                record.ProgressPercentage = pct;
                if (pct >= 100 && record.AchievedDate == null)
                    record.AchievedDate = DateOnly.FromDateTime(DateTime.Today);
                else if (pct < 100)
                    record.AchievedDate = null;
                Console.WriteLine($"[CertProgress]   CertId={certId}: *** UPDATED existing record to {pct}% ***");
            }
        }

        await _context.SaveChangesAsync();
        Console.WriteLine($"[CertProgress] ── done ──");
    }
    public async Task RecalculateFromEnrollmentAsync(int enrollmentId)
    {
        var enrollment = await _context.Enrollments
            .AsNoTracking()
            .FirstOrDefaultAsync(e => e.EnrollmentId == enrollmentId);

        if (enrollment == null) return;
        await RecalculateForTraineeAsync(enrollment.TraineeId);
    }
}
