namespace CourseManagement.ViewModels;

public class AvailabilityIndexViewModel
{
    public int AvailabilityId { get; set; }

    public string InstructorName { get; set; } = string.Empty;

    public DateOnly AvailableDate { get; set; }

    public TimeOnly StartTime { get; set; }

    public TimeOnly EndTime { get; set; }

    public bool IsAvailable { get; set; }
}
