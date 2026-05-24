using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CourseManagement.ViewModels
{
    public class CourseEditViewModel
    {
        public int CourseId { get; set; }

        [Required]
        [Display(Name = "Course Code")]
        public string CourseCode { get; set; }

        [Required]
        [Display(Name = "Course Name")]
        public string CourseName { get; set; }

        public string? Description { get; set; }

        [Required]
        public int Capacity { get; set; }

        [Required]
        [Display(Name = "Enrollment Fee")]
        public decimal EnrollmentFee { get; set; }

        [Required]
        [Display(Name = "Category")]
        public int CategoryId { get; set; }

        public SelectList? Categories { get; set; }
    }
}
