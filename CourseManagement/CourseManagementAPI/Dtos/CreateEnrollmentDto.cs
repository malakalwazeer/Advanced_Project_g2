using System.ComponentModel.DataAnnotations;

namespace CourseManagementAPI.Dtos
{
    public class CreateEnrollmentDto
    {
        [Range(1, int.MaxValue, ErrorMessage = "TraineeId must be greater than 0.")]
        public int TraineeId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "SessionId must be greater than 0.")]
        public int SessionId { get; set; }
    }
}
