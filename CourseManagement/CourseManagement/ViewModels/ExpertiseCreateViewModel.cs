using System.ComponentModel.DataAnnotations;

namespace CourseManagement.ViewModels;

public class ExpertiseCreateViewModel
{
    [Range(1, int.MaxValue, ErrorMessage = "Please select an instructor.")]
    public int InstructorId { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Please select a category.")]
    public int CategoryId { get; set; }
}
