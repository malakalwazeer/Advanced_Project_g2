using System.ComponentModel.DataAnnotations;

namespace CourseManagementAPI.Dtos
{
    public class CertificationLookupDto
    {
        [Required]
        public int TraineeId { get; set; }

        [Required]
        [RegularExpression(@"^CERT-\d+-\d+$", ErrorMessage = "Certificate reference must look like CERT-1-1.")]
        public string CertificateReferenceNumber { get; set; } = null!;
    }
}
