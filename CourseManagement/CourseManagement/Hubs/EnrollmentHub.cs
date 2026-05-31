using Microsoft.AspNetCore.SignalR;

namespace CourseManagement.Hubs;
public class EnrollmentHub : Hub
{
    // Called by the JavaScript client as soon as the Course Details page loads.
    public async Task JoinCourseGroup(int courseId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, GroupName(courseId));
    }

    // Called by the JavaScript client via the browser's beforeunload event.
    // Removing the connection explicitly is good practice even though SignalR
    // cleans up on disconnect, it keeps the group membership accurate while
    // the connection is still alive.
    public async Task LeaveCourseGroup(int courseId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, GroupName(courseId));
    }

    // Centralised group name so hub and service always agree.</summary>
    public static string GroupName(int courseId) => $"course-{courseId}";
}
