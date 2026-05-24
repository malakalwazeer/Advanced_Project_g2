using System.ComponentModel.DataAnnotations;

namespace CourseManagement.ViewModels;

public class PublicCertificationLookupViewModel
{
    [Required(ErrorMessage = "Trainee ID is required.")]
    [Range(1, int.MaxValue, ErrorMessage = "Please enter a valid Trainee ID.")]
    [Display(Name = "Trainee ID")]
    public int TraineeId { get; set; }

    [Required(ErrorMessage = "Certificate reference is required.")]
    [RegularExpression(@"^CERT-\d+-\d+$", ErrorMessage = "Format must be CERT-{TraineeId}-{CertificationId} (e.g. CERT-5-2).")]
    [Display(Name = "Certificate Reference Number")]
    public string CertificateReferenceNumber { get; set; } = string.Empty;

    public bool HasResult { get; set; }
    public bool IsValid { get; set; }
    public string? ErrorMessage { get; set; }
    public string? Message { get; set; }

    public string? TraineeName { get; set; }
    public string? CertificationName { get; set; }
    public int RequiredCoursesCount { get; set; }
    public int CompletedCoursesCount { get; set; }
    public decimal ProgressPercentage { get; set; }
    public List<string> CompletedCourses { get; set; } = new();
    public List<string> MissingCourses { get; set; } = new();
}
