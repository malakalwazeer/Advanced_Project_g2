using CourseManagement.Hubs;
using CourseManagement.ViewModels;
using CourseManagementAPI.Data;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace CourseManagement.Services;

/// Reads the latest enrollment counts from the database after an enrollment
/// change, then pushes the update to every browser tab that is currently
/// viewing that course's Details page.
/// Registered as Scoped so it can safely consume the Scoped DbContext.

public class EnrollmentBroadcastService(
    IHubContext<EnrollmentHub> hubContext,
    CourseManagementDbContext db)
{
    /// Fetches fresh enrollment counts for every session
    /// and broadcasts them to all clients in the course's SignalR group.
    ///
    /// Called after a successful enrollment create (or delete) so connected
    /// clients see the change within milliseconds.
    public async Task BroadcastCourseEnrollmentUpdateAsync(int courseId)
    {
        var sessions = await db.CourseSessions
            .Where(s => s.CourseId == courseId)
            .Select(s => new SessionEnrollmentViewModel
            {
                SessionId     = s.SessionId,
                StartDateTime = s.StartDateTime,
                EndDateTime   = s.EndDateTime,
                Capacity      = s.Capacity,
                EnrolledCount = s.Enrollments.Count()
            })
            .OrderBy(s => s.StartDateTime)
            .ToListAsync();

        // "UpdateEnrollmentCounts" must match the connection.on(...) name in the JS client.
        await hubContext.Clients
            .Group(EnrollmentHub.GroupName(courseId))
            .SendAsync("UpdateEnrollmentCounts", sessions);
    }
}
