using ReportingCourseManagement.Dtos;
namespace ReportingCourseManagement.ViewModels
{
    public class ReportsDashboardViewModel
    {

        //This ViewModel only groups API data for the dashboard page.
        public List<EnrollmentByCourseReportDto> EnrollmentByCourse { get; set; } = new();

        public List<EnrollmentByCategoryReportDto> EnrollmentByCategory { get; set; } = new();

        public List<InstructorWorkloadReportDto> InstructorWorkload { get; set; } = new();

        public RevenueSummaryReportDto RevenueSummary { get; set; } = new();

        public List<CertificationCompletionReportDto> CertificationCompletion { get; set; } = new();
    }
}
