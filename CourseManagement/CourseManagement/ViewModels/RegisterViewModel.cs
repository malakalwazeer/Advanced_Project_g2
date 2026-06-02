using System.ComponentModel.DataAnnotations;

namespace CourseManagement.ViewModels;

public class RegisterViewModel
{
    [Required(ErrorMessage = "Full name is required.")]
    [Display(Name = "Full Name")]
    public string DisplayName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress(ErrorMessage = "Enter a valid email address.")]
    public string Email { get; set; } = string.Empty;

    [StringLength(100)]
    [Display(Name = "Organization")]
    public string? OrganizationName { get; set; }

    [Required]
    public string CountryCode { get; set; } = "+973";

    // --- Updated Mobile Field validation to accept 7 to 9 digits for GCC countries ---
    [Required(ErrorMessage = "Mobile number is required.")]
    [RegularExpression(@"^\d{7,9}$", ErrorMessage = "Please enter a valid mobile number length (7 to 9 digits).")]
    [Display(Name = "Mobile Number")]
    public string Phone { get; set; } = string.Empty;

    [Required(ErrorMessage = "Password is required.")]
    [DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;

    [Required(ErrorMessage = "Confirm password is required.")]
    [DataType(DataType.Password)]
    [Compare(nameof(Password), ErrorMessage = "The password and confirmation password do not match.")]
    [Display(Name = "Confirm password")]
    public string ConfirmPassword { get; set; } = string.Empty;
}