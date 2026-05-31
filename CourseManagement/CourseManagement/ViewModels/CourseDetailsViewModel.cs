using CourseManagementAPI.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CourseManagement.ViewModels;

public class CourseDetailsViewModel
{
    public int CourseId { get; set; }
    public string CourseCode { get; set; } = null!;
    public string CourseName { get; set; } = null!;
    public string? Description { get; set; }
    public int Capacity { get; set; }
    public int DurationHours { get; set; }
    public List<CourseReqEquipment> CurrentRequirements { get; set; } = new();
    public List<CoursePrerequisite> CurrentPrerequisites { get; set; } = new();
    public List<SessionEnrollmentViewModel> Sessions { get; set; } = new();

    //added by malak
    public List<SessionIndexViewModel> AvailableSessions { get; set; } = new();

    // Forms
    public int AddEquipmentId { get; set; }
    public int AddEquipmentQuantity { get; set; }
    public SelectList? EquipmentList { get; set; }

    public int AddPrerequisiteId { get; set; }
    public SelectList? PrerequisiteCourseList { get; set; }
}
