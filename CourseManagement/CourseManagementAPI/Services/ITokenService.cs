using CourseManagementAPI.Models;
namespace CourseManagementAPI.Services
{
    public interface ITokenService
    {
        Task<string> CreateTokenAsync(ApplicationUser user);
    }
}
