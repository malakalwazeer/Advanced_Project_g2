namespace CourseManagement.ViewModels;

public class CertificationIndexViewModel
{
    public int CertificationId { get; set; }
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public int CourseCount { get; set; }
}
