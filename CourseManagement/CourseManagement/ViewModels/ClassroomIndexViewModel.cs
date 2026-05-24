namespace CourseManagement.ViewModels;

public class ClassroomIndexViewModel
{
    public int ClassroomId { get; set; }

    public string Location { get; set; } = string.Empty;

    public int Capacity { get; set; }

    public bool IsActive { get; set; }
}
