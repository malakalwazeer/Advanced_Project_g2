using Microsoft.AspNetCore.Identity;
namespace CourseManagementAPI.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string? DisplayName { get; set; } = string.Empty;
    }
}
