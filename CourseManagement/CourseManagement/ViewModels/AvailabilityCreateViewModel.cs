using System.ComponentModel.DataAnnotations;

namespace CourseManagement.ViewModels;

public class AvailabilityCreateViewModel
{
    [Range(1, int.MaxValue, ErrorMessage = "Please select an instructor.")]
    public int InstructorId { get; set; }

    [Required]
    [DataType(DataType.Date)]
    public DateOnly AvailableDate { get; set; } = DateOnly.FromDateTime(DateTime.Today);

    [Required]
    [DataType(DataType.Time)]
    public TimeOnly StartTime { get; set; }

    [Required]
    [DataType(DataType.Time)]
    public TimeOnly EndTime { get; set; }

    [Display(Name = "Available")]
    public bool IsAvailable { get; set; } = true;
}
