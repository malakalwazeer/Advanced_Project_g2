namespace CourseManagement.ViewModels;

public class TraineeDetailsViewModel
{
    public int TraineeId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string? OrganizationName { get; set; }
    public DateOnly RegistrationDate { get; set; }
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string? StatusName { get; set; }

    public List<TraineeCertProgressRow> CertificationProgresses { get; set; } = new();
    public List<TraineeEnrollmentRow> Enrollments { get; set; } = new();
}

public class TraineeCertProgressRow
{
    public string? CertificationName { get; set; }
    public decimal ProgressPercentage { get; set; }
    public DateOnly? AchievedDate { get; set; }
}

public class TraineeEnrollmentRow
{
    public string? CourseName { get; set; }
    public DateTime? SessionStart { get; set; }
    public DateTime? SessionEnd { get; set; }
    public DateOnly EnrollmentDate { get; set; }
    public string? StatusName { get; set; }
}
