public interface ICertificateService
{
    byte[] GenerateCertificate(
        string traineeName,
        string certificationName,
        string certificateId,
        string verifyUrl,
        DateOnly? achievedDate);
}