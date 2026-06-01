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
    public async Task<List<SessionEnrollmentViewModel>> GetSessionSnapshotsAsync(int courseId)
    {
        // all sessions for this course.
        var sessions = await db.CourseSessions
            .Where(s => s.CourseId == courseId)
            .OrderBy(s => s.StartDateTime)
            .Select(s => new { s.SessionId, s.StartDateTime, s.EndDateTime, s.Capacity })
            .ToListAsync();

        var sessionIds = sessions.Select(s => s.SessionId).ToList();

        // count only active-status enrollment count per session.
        // Dropped/Completed are excluded
        var rawCounts = await db.Enrollments
            .Where(e => sessionIds.Contains(e.SessionId)
                && ActiveStatuses.Contains(e.EnrollmentStatus.StatusName))
            .GroupBy(e => e.SessionId)
            .Select(g => new { SessionId = g.Key, Count = g.Count() })
            .ToListAsync();

        var countsBySession = rawCounts.ToDictionary(x => x.SessionId, x => x.Count);

        return sessions.Select(s => new SessionEnrollmentViewModel
        {
            SessionId     = s.SessionId,
            StartDateTime = s.StartDateTime,
            EndDateTime   = s.EndDateTime,
            Capacity      = s.Capacity,
            EnrolledCount = countsBySession.GetValueOrDefault(s.SessionId, 0)
        }).ToList();
    }
    public async Task BroadcastCourseEnrollmentUpdateAsync(int courseId)
    {
        var snapshots = await GetSessionSnapshotsAsync(courseId);

        Console.WriteLine($"[SignalR] Clients.Group → course-{courseId} | {snapshots.Count} sessions");
        foreach (var snap in snapshots)
            Console.WriteLine($"[SignalR]   session={snap.SessionId} " +
                $"active={snap.EnrolledCount}/{snap.Capacity} " +
                $"remaining={snap.RemainingSpots} isFull={snap.IsFull}");

        // Delivers to everyone who called JoinCourseGroup(courseId).
        await hubContext.Clients
            .Group($"course-{courseId}")
            .SendAsync("EnrollmentUpdated", snapshots);
    }
    public async Task NotifyTraineeAsync(int traineeId, string courseName, string newStatus)
    {
        var trainee = await db.Trainees
            .AsNoTracking()
            .FirstOrDefaultAsync(t => t.TraineeId == traineeId);

        if (trainee == null) return;

        var appUser = await db.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Email == trainee.Email);

        if (appUser == null) return;

        var payload = new
        {
            message   = $"Your enrollment in '{courseName}' has been updated to '{newStatus}'.",
            courseName,
            status    = newStatus
        };

        Console.WriteLine($"[SignalR] Clients.User → userId={appUser.Id} | {payload.message}");

        await hubContext.Clients.User(appUser.Id)
            .SendAsync("YourEnrollmentUpdated", payload);
    }
}
