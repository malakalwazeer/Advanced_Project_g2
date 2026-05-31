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
    public async Task BroadcastCourseEnrollmentUpdateAsync(int courseId)
    {
        // Fetch the latest enrollment snapshot for every session of this course.
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

        // Push to Clients.Group, only clients that have called JoinCourseGroup
        // for this courseId will receive the "EnrollmentUpdated" event.
        await hubContext.Clients
            .Group($"course-{courseId}")
            .SendAsync("EnrollmentUpdated", sessions);
    }
}
