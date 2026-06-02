using System.ComponentModel.DataAnnotations;

namespace CourseManagement.ViewModels;

public class InstructorCreateViewModel
{
    [Required(ErrorMessage = "Full name is required.")]
    [Display(Name = "Full Name")]
    public string FullName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress(ErrorMessage = "Enter a valid email address.")]
    public string Email { get; set; } = string.Empty;

    [Required]
    public string CountryCode { get; set; } = "+973"; // Default to Bahrain

    [Required(ErrorMessage = "Mobile number is required.")]
    [RegularExpression(@"^\d{7,9}$", ErrorMessage = "Please enter a valid mobile number length (7 to 9 digits).")]
    [Display(Name = "Mobile Number")]
    public string Phone { get; set; } = string.Empty;

    public string? Qualifications { get; set; }

    [Required(ErrorMessage = "Hire date is required.")]
    [DataType(DataType.Date)]
    [Display(Name = "Hire Date")]
    public DateTime HireDate { get; set; } = DateTime.Today;
}