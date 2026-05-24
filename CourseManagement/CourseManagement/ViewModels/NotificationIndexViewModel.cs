namespace CourseManagement.ViewModels;

public class NotificationIndexViewModel
{
    public int NotificationId { get; set; }
    public string Title { get; set; } = null!;
    public string Message { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
}