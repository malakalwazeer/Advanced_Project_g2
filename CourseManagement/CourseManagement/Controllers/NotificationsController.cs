using CourseManagementAPI.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CourseManagement.Controllers;

[Authorize(Roles = "TrainingCoordinator,Instructor,Trainee")] //added by malak
public class NotificationsController : Controller
{
    private readonly CourseManagementDbContext _context;

    public NotificationsController(CourseManagementDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var notifications = await _context.Notifications
            .Include(n => n.Session)
            .OrderByDescending(n => n.NotificationId)
            .ToListAsync();

        return View(notifications);
    }
}