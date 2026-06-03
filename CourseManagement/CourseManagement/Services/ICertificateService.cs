using CourseManagementAPI.Models;

namespace CourseManagement.Services;

public interface ICertificateService
{
    byte[] GenerateCertificate(string traineeName, string certificationName, DateOnly? achievedDate);
}
