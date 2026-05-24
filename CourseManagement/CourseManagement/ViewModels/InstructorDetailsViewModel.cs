using CourseManagementAPI.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CourseManagement.ViewModels;

public class InstructorDetailsViewModel
{
    public int InstructorId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Qualifications { get; set; } = string.Empty;

    public List<InstructorExpertise> Expertises { get; set; } = new();
    public List<InstructorAvailability> Availabilities { get; set; } = new();

    // Expertise form
    public int AddCategoryId { get; set; }
    public SelectList? CategoryList { get; set; }

    // Availability form
    public DateTime AddAvailableDate { get; set; } = DateTime.Today;
    public TimeSpan AddStartTime { get; set; }
    public TimeSpan AddEndTime { get; set; }
}
