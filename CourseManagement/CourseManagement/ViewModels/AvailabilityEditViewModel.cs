using System.ComponentModel.DataAnnotations;

namespace CourseManagement.ViewModels
{
    public class AvailabilityEditViewModel
    {
        public int AvailabilityId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Please select an instructor.")]
        public int InstructorId { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateOnly AvailableDate { get; set; }

        [Required]
        [DataType(DataType.Time)]
        public TimeOnly StartTime { get; set; }

        [Required]
        [DataType(DataType.Time)]
        public TimeOnly EndTime { get; set; }

        [Display(Name = "Available")]
        public bool IsAvailable { get; set; }
    }
}
