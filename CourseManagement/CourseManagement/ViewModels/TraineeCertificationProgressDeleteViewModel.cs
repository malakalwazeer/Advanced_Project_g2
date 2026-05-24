namespace CourseManagement.ViewModels;

public class TraineeCertificationProgressDeleteViewModel
{
    public int TraineeId { get; set; }
    public int CertificationId { get; set; }
    public string? TraineeName { get; set; }
    public string? CertificationName { get; set; }
    public decimal ProgressPercentage { get; set; }
    public DateOnly? AchievedDate { get; set; }
}
