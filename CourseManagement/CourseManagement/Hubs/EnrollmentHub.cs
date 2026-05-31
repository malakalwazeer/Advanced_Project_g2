using Microsoft.AspNetCore.SignalR;

namespace CourseManagement.Hubs;
public class EnrollmentHub : Hub
{
    public override async Task OnConnectedAsync()
    {
        await base.OnConnectedAsync();
    }

    public async Task JoinCourseGroup(int courseId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, $"course-{courseId}");
    }

    public async Task LeaveCourseGroup(int courseId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"course-{courseId}");
    }
}
