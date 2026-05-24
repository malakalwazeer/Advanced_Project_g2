namespace CourseManagement.ViewModels;

public class TraineeIndexViewModel
{
    public int TraineeId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string? OrganizationName { get; set; }
    public DateOnly RegistrationDate { get; set; }
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string? StatusName { get; set; }
}
