using System.ComponentModel.DataAnnotations;

namespace CourseManagement.ViewModels;

public class ClassroomEditViewModel
{
    public int ClassroomId { get; set; }

    [Required]
    public string Location { get; set; } = string.Empty;

    [Range(1, int.MaxValue, ErrorMessage = "Capacity must be greater than zero.")]
    public int Capacity { get; set; }

    [Display(Name = "Active")]
    public bool IsActive { get; set; }
}
