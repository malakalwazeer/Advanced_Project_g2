namespace CourseManagementAPI.Dtos.Reports
{
    public class EnrollmentByCategoryReportDto
    {
        public string CategoryName { get; set; } = null!;
        public int EnrollmentCount { get; set; }
    }
}
