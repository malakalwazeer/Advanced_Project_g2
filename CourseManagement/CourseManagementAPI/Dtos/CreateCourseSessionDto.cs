using System.ComponentModel.DataAnnotations;
namespace CourseManagementAPI.Dtos
{
    public class CreateCourseSessionDto
    {
        [Required]
        public int InstructorId { get; set; }

        [Required]
        public int CourseId { get; set; }

        [Required]
        public int ClassroomId { get; set; }

        [Required]
        public DateTime StartDateTime { get; set; }

        [Required]
        public DateTime EndDateTime { get; set; }

        [Range(1, 100, ErrorMessage = "Capacity must be between 1 and 100.")]
        public int Capacity { get; set; }
    }
}
