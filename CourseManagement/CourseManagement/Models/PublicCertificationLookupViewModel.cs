namespace CourseManagement.Models.ViewModels;

public class PublicCertificationLookupViewModel
{
    public int TraineeId { get; set; }
    public string CertificateReferenceNumber { get; set; } = string.Empty;

    public bool HasResult { get; set; }
    public bool IsValid { get; set; }
    public string? ErrorMessage { get; set; }

    public string? TraineeName { get; set; }
    public string? CertificationName { get; set; }
    public int RequiredCoursesCount { get; set; }
    public int CompletedCoursesCount { get; set; }
    public decimal ProgressPercentage { get; set; }
    public List<string> CompletedCourses { get; set; } = new();
    public List<string> MissingCourses { get; set; } = new();
    public string? Message { get; set; }
}
