using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CourseManagement.ViewModels;

public class AssessmentCreateEditViewModel
{
    public int AssessmentId { get; set; }

    [Required(ErrorMessage = "Enrollment is required.")]
    [Range(1, int.MaxValue, ErrorMessage = "Please select an enrollment.")]
    [Display(Name = "Enrollment")]
    public int EnrollmentId { get; set; }

    [Required(ErrorMessage = "Instructor is required.")]
    [Range(1, int.MaxValue, ErrorMessage = "Please select an instructor.")]
    [Display(Name = "Instructor")]
    public int InstructorId { get; set; }

    [Display(Name = "Score (0–100)")]
    [Range(0, 100, ErrorMessage = "Score must be between 0 and 100.")]
    public int? Score { get; set; }

    [Display(Name = "Result")]
    [Range(0, 1, ErrorMessage = "Result must be 0 (Fail) or 1 (Pass).")]
    public int? Result { get; set; }

    public List<SelectListItem> Enrollments { get; set; } = new();
    public List<SelectListItem> Instructors { get; set; } = new();
}
