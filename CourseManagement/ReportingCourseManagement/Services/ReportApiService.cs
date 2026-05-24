using ReportingCourseManagement.Dtos;

namespace ReportingCourseManagement.Services
{
    public class ReportApiService
    {
        private readonly HttpClient _httpClient;

        public ReportApiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<EnrollmentByCourseReportDto>> GetEnrollmentByCourseAsync()
        {
            return await _httpClient.GetFromJsonAsync<List<EnrollmentByCourseReportDto>>(
                "api/reports/enrollment-by-course") ?? new List<EnrollmentByCourseReportDto>();
        }

        public async Task<List<EnrollmentByCategoryReportDto>> GetEnrollmentByCategoryAsync()
        {
            return await _httpClient.GetFromJsonAsync<List<EnrollmentByCategoryReportDto>>(
                "api/reports/enrollment-by-category") ?? new List<EnrollmentByCategoryReportDto>();
        }

        public async Task<List<InstructorWorkloadReportDto>> GetInstructorWorkloadAsync()
        {
            return await _httpClient.GetFromJsonAsync<List<InstructorWorkloadReportDto>>(
                "api/reports/instructor-workload") ?? new List<InstructorWorkloadReportDto>();
        }

        public async Task<RevenueSummaryReportDto?> GetRevenueSummaryAsync()
        {
            return await _httpClient.GetFromJsonAsync<RevenueSummaryReportDto>(
                "api/reports/revenue-summary");
        }

        public async Task<List<CertificationCompletionReportDto>> GetCertificationCompletionAsync()
        {
            return await _httpClient.GetFromJsonAsync<List<CertificationCompletionReportDto>>(
                "api/reports/certification-completion") ?? new List<CertificationCompletionReportDto>();
        }
    }
}
