using System.ComponentModel.DataAnnotations;

namespace CourseManagement.ViewModels;

public class CertificationProgressViewModel
{
    public int TraineeId { get; set; }
    public int CertificationId { get; set; }

    [Display(Name = "Trainee")]
    public string? TraineeName { get; set; }

    [Display(Name = "Certification")]
    public string? CertificationName { get; set; }

    [Display(Name = "Progress")]
    public decimal ProgressPercentage { get; set; }

    [Display(Name = "Achieved Date")]
    public DateOnly? AchievedDate { get; set; }
}
