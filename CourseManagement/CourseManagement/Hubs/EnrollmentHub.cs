using CourseManagementAPI.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace CourseManagement.Hubs;

[Authorize]
public class EnrollmentHub : Hub
{
    private readonly CourseManagementDbContext _db;

    public EnrollmentHub(CourseManagementDbContext db)
    {
        _db = db;
    }

    public override async Task OnConnectedAsync()
    {
        var username = Context.User?.Identity?.Name ?? "anonymous";
        Console.WriteLine($"[Hub] Connected  : {username} | id={Context.ConnectionId}");
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var username = Context.User?.Identity?.Name ?? "anonymous";
        Console.WriteLine($"[Hub] Disconnected: {username} | id={Context.ConnectionId}");
        await base.OnDisconnectedAsync(exception);
    }

    public async Task JoinCourseGroup(int courseId)
    {
        if (!Context.User!.IsInRole("TrainingCoordinator") &&
            !Context.User.IsInRole("Trainee"))
        {
            throw new HubException(
                "Access denied: you do not have permission to view this course.");
        }

        var courseExists = await _db.Courses.AnyAsync(c => c.CourseId == courseId);
        if (!courseExists)
        {
            throw new HubException($"Course {courseId} does not exist.");
        }

        await Groups.AddToGroupAsync(Context.ConnectionId, $"course-{courseId}");
        Console.WriteLine($"[Hub] Joined group: {Context.User.Identity?.Name} → course-{courseId}");
    }

    public async Task LeaveCourseGroup(int courseId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"course-{courseId}");
        Console.WriteLine($"[Hub] Left group : {Context.User?.Identity?.Name} ← course-{courseId}");
    }
}
