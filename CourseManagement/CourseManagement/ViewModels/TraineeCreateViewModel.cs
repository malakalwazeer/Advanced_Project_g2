using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CourseManagement.ViewModels;

public class TraineeCreateViewModel
{
    [Required(ErrorMessage = "Full name is required.")]
    [StringLength(100)]
    [Display(Name = "Full Name")]
    public string FullName { get; set; } = string.Empty;

    [StringLength(100)]
    [Display(Name = "Organization")]
    public string? OrganizationName { get; set; }

    [Required(ErrorMessage = "Registration date is required.")]
    [Display(Name = "Registration Date")]
    public DateOnly RegistrationDate { get; set; }

    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress(ErrorMessage = "Enter a valid email address.")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Phone is required.")]
    [RegularExpression(@"^\d{8}$", ErrorMessage = "Phone must be exactly 8 digits.")]
    public string Phone { get; set; } = string.Empty;

    [Required(ErrorMessage = "Password is required.")]
    [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be at least 6 characters.")]
    [DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;

    [Required(ErrorMessage = "Status is required.")]
    [Range(1, int.MaxValue, ErrorMessage = "Please select a status.")]
    [Display(Name = "Status")]
    public int TraineeStatusId { get; set; }

    public List<SelectListItem> TraineeStatuses { get; set; } = new();
}
