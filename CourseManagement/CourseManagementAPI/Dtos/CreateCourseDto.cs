using System.ComponentModel.DataAnnotations;

namespace CourseManagementAPI.Dtos
{
    public class CreateCourseDto
    {
        [Required(ErrorMessage = "Course code is required.")]
        [StringLength(20, ErrorMessage = "Course code cannot exceed 20 characters.")]
        public string CourseCode { get; set; } = null!;

        [Required(ErrorMessage = "Course name is required.")]
        [StringLength(150, ErrorMessage = "Course name cannot exceed 150 characters.")]
        public string CourseName { get; set; } = null!;

        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters.")]
        public string? Description { get; set; }

        [Range(1, 100, ErrorMessage = "Capacity must be between 1 and 100.")]
        public int Capacity { get; set; }

        [Range(0, 10000, ErrorMessage = "Enrollment fee must be 0 or more.")]
        public decimal EnrollmentFee { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "CategoryId must be greater than 0.")]
        public int CategoryId { get; set; }

        [Range(1, 200, ErrorMessage = "Duration hours must be between 1 and 200.")]
        public int DurationHours { get; set; }
    }
}
