using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CourseManagement.ViewModels;

public class CourseCreateViewModel
{
    [Required]
    [Display(Name = "Course Code")]
    public string CourseCode { get; set; } = string.Empty;

    [Required]
    [Display(Name = "Title")]
    public string CourseName { get; set; } = string.Empty;

    public string? Description { get; set; }

    [Display(Name = "Duration (Hours)")]
    [Range(1, int.MaxValue, ErrorMessage = "Duration must be at least 1 hour.")]
    public int DurationHours { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Capacity must be greater than zero.")]
    public int Capacity { get; set; }

    [Range(0.01, double.MaxValue, ErrorMessage = "Fee must be greater than zero.")]
    [Display(Name = "Fee")]
    public decimal EnrollmentFee { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Please select a category.")]
    [Display(Name = "Category")]
    public int CategoryId { get; set; }

    public SelectList? Categories { get; set; }
}
