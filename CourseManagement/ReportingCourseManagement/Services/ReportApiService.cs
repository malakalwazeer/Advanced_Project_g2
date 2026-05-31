using ReportingCourseManagement.Dtos;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace ReportingCourseManagement.Services
{
    public class ReportApiService
    {
        private readonly HttpClient _httpClient;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ReportApiService(HttpClient httpClient, IHttpContextAccessor httpContextAccessor)
        {
            _httpClient = httpClient;
            _httpContextAccessor = httpContextAccessor; 
        }

        private void AttachBearerToken()
        {
            _httpClient.DefaultRequestHeaders.Authorization = null;

            var token = _httpContextAccessor.HttpContext?.Session.GetString("JWToken");

            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", token);
            }
        }

        public async Task<List<EnrollmentByCourseReportDto>> GetEnrollmentByCourseAsync()
        {
            AttachBearerToken();
            try
            {
                return await _httpClient.GetFromJsonAsync<List<EnrollmentByCourseReportDto>>(
                    "api/reports/enrollment-by-course") ?? new List<EnrollmentByCourseReportDto>();
            }
            catch (HttpRequestException)
            {
                // Catch unauthorized or API connection down states gracefully
                return new List<EnrollmentByCourseReportDto>();
            }
        }

        public async Task<List<EnrollmentByCategoryReportDto>> GetEnrollmentByCategoryAsync()
        {
            AttachBearerToken();
            try
            {
                return await _httpClient.GetFromJsonAsync<List<EnrollmentByCategoryReportDto>>(
                    "api/reports/enrollment-by-category") ?? new List<EnrollmentByCategoryReportDto>();
            }
            catch (HttpRequestException)
            {
                return new List<EnrollmentByCategoryReportDto>();
            }
        }

        public async Task<List<InstructorWorkloadReportDto>> GetInstructorWorkloadAsync()
        {
            AttachBearerToken();
            try
            {
                return await _httpClient.GetFromJsonAsync<List<InstructorWorkloadReportDto>>(
                    "api/reports/instructor-workload") ?? new List<InstructorWorkloadReportDto>();
            }
            catch (HttpRequestException)
            {
                return new List<InstructorWorkloadReportDto>();
            }
        }

        public async Task<RevenueSummaryReportDto?> GetRevenueSummaryAsync()
        {
            AttachBearerToken();
            try
            {
                return await _httpClient.GetFromJsonAsync<RevenueSummaryReportDto>(
                    "api/reports/revenue-summary");
            }
            catch (HttpRequestException)
            {
                return null;
            }
        }

        public async Task<List<CertificationCompletionReportDto>> GetCertificationCompletionAsync()
        {
            AttachBearerToken();
            try
            {
                return await _httpClient.GetFromJsonAsync<List<CertificationCompletionReportDto>>(
                    "api/reports/certification-completion") ?? new List<CertificationCompletionReportDto>();
            }
            catch (HttpRequestException)
            {
                return new List<CertificationCompletionReportDto>();
            }
        }
    }
}