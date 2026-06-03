using Microsoft.AspNetCore.Mvc.Rendering;

namespace CourseManagement.ViewModels;

public class CertificationDetailsViewModel
{
    public int CertificationId { get; set; }
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public List<CertificationCourseRow> LinkedCourses { get; set; } = new();

    public int AddCourseId { get; set; }
    public SelectList? AvailableCourses { get; set; }
}

public class CertificationCourseRow
{
    public int CourseId { get; set; }
    public string CourseName { get; set; } = null!;
    public string? CourseCode { get; set; }
}
