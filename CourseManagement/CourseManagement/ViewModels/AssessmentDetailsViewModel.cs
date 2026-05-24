namespace CourseManagement.ViewModels;

public class AssessmentDetailsViewModel
{
    public int AssessmentId { get; set; }
    public string? TraineeName { get; set; }
    public string? CourseName { get; set; }
    public DateTime? SessionStart { get; set; }
    public string? InstructorName { get; set; }
    public int? Score { get; set; }
    public int? Result { get; set; }
}
