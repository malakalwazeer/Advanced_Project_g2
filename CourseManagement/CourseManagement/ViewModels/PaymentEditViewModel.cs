using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CourseManagement.ViewModels;

public class PaymentEditViewModel
{
    public int PaymentId { get; set; }

    [Required(ErrorMessage = "Enrollment is required.")]
    [Range(1, int.MaxValue, ErrorMessage = "Please select an enrollment.")]
    [Display(Name = "Enrollment")]
    public int EnrollmentId { get; set; }

    [Required(ErrorMessage = "Amount paid is required.")]
    [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than zero.")]
    [Display(Name = "Amount Paid")]
    public decimal AmountPaid { get; set; }

    [Required(ErrorMessage = "Payment date is required.")]
    [Display(Name = "Payment Date")]
    public DateOnly PaymentDate { get; set; }

    public List<SelectListItem> Enrollments { get; set; } = new();
}
