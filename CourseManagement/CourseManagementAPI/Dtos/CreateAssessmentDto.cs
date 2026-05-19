using System.ComponentModel.DataAnnotations;

namespace CourseManagementAPI.Dtos
{
    public class CreateAssessmentDto
    {
        [Range(1, int.MaxValue, ErrorMessage = "EnrollmentId must be greater than 0.")]
        public int EnrollmentId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "InstructorId must be greater than 0.")]
        public int InstructorId { get; set; }

        [Range(0, 1, ErrorMessage = "Result must be 0 for Fail or 1 for Pass.")]
        public int Result { get; set; }

        [Range(0, 100, ErrorMessage = "Score must be between 0 and 100.")]
        public int? Score { get; set; }
    }
}
