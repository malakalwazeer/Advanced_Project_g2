using System.ComponentModel.DataAnnotations;

namespace CourseManagement.ViewModels;

public class InstructorEditViewModel
{
    public int InstructorId { get; set; }

    [Required]
    [Display(Name = "Full Name")]
    public string FullName { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    [Phone]
    public string Phone { get; set; } = string.Empty;

    public string? Qualifications { get; set; }

    [Required]
    [DataType(DataType.Date)]
    [Display(Name = "Hire Date")]
    public DateTime HireDate { get; set; }
}
