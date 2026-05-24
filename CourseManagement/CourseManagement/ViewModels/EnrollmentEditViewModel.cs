using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CourseManagement.ViewModels;

public class EnrollmentEditViewModel
{
    public int EnrollmentId { get; set; }

    [Required(ErrorMessage = "Trainee is required.")]
    [Range(1, int.MaxValue, ErrorMessage = "Please select a trainee.")]
    [Display(Name = "Trainee")]
    public int TraineeId { get; set; }

    [Required(ErrorMessage = "Session is required.")]
    [Range(1, int.MaxValue, ErrorMessage = "Please select a session.")]
    [Display(Name = "Session")]
    public int SessionId { get; set; }

    [Required(ErrorMessage = "Enrollment date is required.")]
    [Display(Name = "Enrollment Date")]
    public DateOnly EnrollmentDate { get; set; }

    [Required(ErrorMessage = "Status is required.")]
    [Range(1, int.MaxValue, ErrorMessage = "Please select a status.")]
    [Display(Name = "Status")]
    public int EnrollmentStatusId { get; set; }

    public List<SelectListItem> Trainees { get; set; } = new();
    public List<SelectListItem> Sessions { get; set; } = new();
    public List<SelectListItem> EnrollmentStatuses { get; set; } = new();
}
