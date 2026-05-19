using System.ComponentModel.DataAnnotations;

namespace CourseManagementAPI.Dtos
{
    public class CreatePaymentDto
    {
        [Range(1, int.MaxValue, ErrorMessage = "EnrollmentId must be greater than 0.")]
        public int EnrollmentId { get; set; }

        [Range(0.01, 10000, ErrorMessage = "Amount paid must be greater than 0.")]
        public decimal AmountPaid { get; set; }

        [Required]
        public DateOnly PaymentDate { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "PaymentStatusId must be greater than 0.")]
        public int PaymentStatusId { get; set; }
    }
}
