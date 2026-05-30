using System.ComponentModel.DataAnnotations;

namespace CourseManagementAPI.Dtos
{
    public class RegisterDto
    {
        [Required, EmailAddress]
        public required string Email { get; set; }

        [Required, StringLength(50, MinimumLength = 2)]
        public required string DisplayName { get; set; }

        [Required, StringLength(100, MinimumLength = 6)]
        public required string Password { get; set; }

        [Required]
        public required string Role { get; set; }
    }
}
