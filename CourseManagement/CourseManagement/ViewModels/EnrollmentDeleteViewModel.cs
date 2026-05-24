namespace CourseManagement.ViewModels;

public class EnrollmentDeleteViewModel
{
    public int EnrollmentId { get; set; }
    public string? TraineeName { get; set; }
    public string? CourseName { get; set; }
    public DateTime? SessionStart { get; set; }
    public DateOnly EnrollmentDate { get; set; }
    public string? StatusName { get; set; }
}
