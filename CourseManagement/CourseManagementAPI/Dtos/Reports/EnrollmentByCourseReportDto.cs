namespace CourseManagementAPI.Dtos.Reports
{
    public class EnrollmentByCourseReportDto
    {
        public string CourseName { get; set; } = null!;
        public int EnrollmentCount { get; set; }
    }
}
