using System.ComponentModel.DataAnnotations;

namespace CourseManagement.ViewModels;

public class ProfileSettingsViewModel
{
    // Profile Read-Only/Information Fields
    public string Email { get; set; } = string.Empty;

    [Display(Name = "Display Name")]
    public string DisplayName { get; set; } = string.Empty;

    public string ActiveRole { get; set; } = string.Empty;

    // Password Update Section
    [Required(ErrorMessage = "Old password is required.")]
    [DataType(DataType.Password)]
    [Display(Name = "Old Password")]
    public string OldPassword { get; set; } = string.Empty;

    [Required(ErrorMessage = "New password is required.")]
    [StringLength(100, MinimumLength = 6, ErrorMessage = "The new password must be at least 6 characters long.")]
    [DataType(DataType.Password)]
    [Display(Name = "New Password")]
    public string NewPassword { get; set; } = string.Empty;

    [DataType(DataType.Password)]
    [Display(Name = "Confirm New Password")]
    [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
    public string ConfirmNewPassword { get; set; } = string.Empty;
}