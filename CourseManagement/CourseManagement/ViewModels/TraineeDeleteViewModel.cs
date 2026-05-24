namespace CourseManagement.ViewModels;

public class TraineeDeleteViewModel
{
    public int TraineeId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string? OrganizationName { get; set; }
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string? StatusName { get; set; }
}
