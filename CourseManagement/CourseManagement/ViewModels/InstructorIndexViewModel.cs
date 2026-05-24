namespace CourseManagement.ViewModels;

public class InstructorIndexViewModel
{
    public int InstructorId { get; set; }

    public string FullName { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string Phone { get; set; } = string.Empty;

    public string? Qualifications { get; set; }
}
