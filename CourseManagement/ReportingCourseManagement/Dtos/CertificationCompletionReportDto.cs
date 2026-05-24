namespace ReportingCourseManagement.Dtos
{
    public class CertificationCompletionReportDto
    {
        public string CertificationName { get; set; } = null!;
        public int CompletedCount { get; set; }
        public int InProgressCount { get; set; }
    }
}
