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
        var passedCourseIds = await _context.Assessments
            .Include(a => a.Enrollment)
                .ThenInclude(e => e.Session)
            .Where(a => a.Enrollment.TraineeId == traineeId && a.Result == 1)
            .Select(a => a.Enrollment.Session.CourseId)
            .Distinct()
            .ToListAsync();

        var certIdsFromPassed = await _context.CertificationCourses
            .Where(cc => passedCourseIds.Contains(cc.CourseId))
            .Select(cc => cc.CertificationId)
            .Distinct()
            .ToListAsync();

        var certIdsWithRecord = await _context.TraineeCertificationProgresses
            .Where(p => p.TraineeId == traineeId)
            .Select(p => p.CertificationId)
            .ToListAsync();

        var certIds = certIdsFromPassed.Union(certIdsWithRecord).ToList();

        foreach (var certId in certIds)
        {
            var requiredCourseIds = await _context.CertificationCourses
                .Where(cc => cc.CertificationId == certId && cc.IsRequired)
                .Select(cc => cc.CourseId)
                .ToListAsync();

            if (requiredCourseIds.Count == 0) continue;

            int passedCount = requiredCourseIds.Count(id => passedCourseIds.Contains(id));
            decimal pct = Math.Round((decimal)passedCount / requiredCourseIds.Count * 100, 2);

            var record = await _context.TraineeCertificationProgresses
                .FirstOrDefaultAsync(p => p.TraineeId == traineeId && p.CertificationId == certId);

            if (record == null)
            {
                if (pct == 0) continue;
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
                record.ProgressPercentage = pct;
                if (pct >= 100 && record.AchievedDate == null)
                    record.AchievedDate = DateOnly.FromDateTime(DateTime.Today);
                else if (pct < 100)
                    record.AchievedDate = null;
            }
        }

        await _context.SaveChangesAsync();
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
