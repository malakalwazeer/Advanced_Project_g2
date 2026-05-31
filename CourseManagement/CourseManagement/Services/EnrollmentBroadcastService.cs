using CourseManagement.Hubs;
using CourseManagement.ViewModels;
using CourseManagementAPI.Data;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace CourseManagement.Services;
public class EnrollmentBroadcastService(
    IHubContext<EnrollmentHub> hubContext,
    CourseManagementDbContext db)
{
    private static readonly string[] ActiveStatuses =
        ["Enrolled", "Confirmed", "Attending"];

    public async Task BroadcastCourseEnrollmentUpdateAsync(int courseId)
    {
        // all sessions for this course.
        var sessions = await db.CourseSessions
            .Where(s => s.CourseId == courseId)
            .OrderBy(s => s.StartDateTime)
            .Select(s => new { s.SessionId, s.StartDateTime, s.EndDateTime, s.Capacity })
            .ToListAsync();

        var sessionIds = sessions.Select(s => s.SessionId).ToList();

        // count only active-status enrollments, grouped by session.
        // Dropped/Completed are excluded
        var rawCounts = await db.Enrollments
            .Where(e => sessionIds.Contains(e.SessionId)
                && ActiveStatuses.Contains(e.EnrollmentStatus.StatusName))
            .GroupBy(e => e.SessionId)
            .Select(g => new { SessionId = g.Key, Count = g.Count() })
            .ToListAsync();

        var countsBySession = rawCounts.ToDictionary(x => x.SessionId, x => x.Count);

        var snapshots = sessions.Select(s => new SessionEnrollmentViewModel
        {
            SessionId     = s.SessionId,
            StartDateTime = s.StartDateTime,
            EndDateTime   = s.EndDateTime,
            Capacity      = s.Capacity,
            EnrolledCount = countsBySession.GetValueOrDefault(s.SessionId, 0)
        }).ToList();

        Console.WriteLine($"[SignalR] Broadcasting EnrollmentUpdated → courseId={courseId} ({snapshots.Count} sessions)");
        foreach (var snap in snapshots)
            Console.WriteLine($"[SignalR]   session={snap.SessionId} " +
                $"active={snap.EnrolledCount}/{snap.Capacity} " +
                $"remaining={snap.RemainingSpots} isFull={snap.IsFull}");

        await hubContext.Clients
            .Group($"course-{courseId}")
            .SendAsync("EnrollmentUpdated", snapshots);
    }
}
