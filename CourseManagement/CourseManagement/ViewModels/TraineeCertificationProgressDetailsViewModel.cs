namespace CourseManagement.ViewModels;

public class TraineeCertificationProgressDetailsViewModel
{
    public int TraineeId { get; set; }
    public int CertificationId { get; set; }
    public string? TraineeName { get; set; }
    public string? CertificationName { get; set; }
    public decimal ProgressPercentage { get; set; }
    public DateOnly? AchievedDate { get; set; }
    public int RequiredCoursesCount { get; set; }
    public int PassedCoursesCount { get; set; }
    public List<CertCourseRow> CertificationCourses { get; set; } = new();
}

public class CertCourseRow
{
    public string? CourseName { get; set; }
    public bool IsRequired { get; set; }
}
