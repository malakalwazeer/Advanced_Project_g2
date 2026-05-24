using System.ComponentModel.DataAnnotations;

namespace CourseManagement.ViewModels;

public class ClassroomCreateViewModel
{
    [Required]
    public string Location { get; set; } = string.Empty;

    [Range(1, int.MaxValue, ErrorMessage = "Capacity must be greater than zero.")]
    public int Capacity { get; set; }

    [Display(Name = "Active")]
    public bool IsActive { get; set; } = true;
}
