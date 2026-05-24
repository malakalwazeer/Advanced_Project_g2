using System.ComponentModel.DataAnnotations;

namespace CourseManagement.ViewModels
{
    public class CourseReqEqEditViewModel
    {
        [Required]
        [Display(Name = "Course")]
        public int CourseId { get; set; }

        [Required]
        [Display(Name = "Equipment")]
        public int EquipmentId { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1.")]
        public int Quantity { get; set; }
        
        // Names for display only in the Edit view header
        public string? CourseName { get; set; }
        public string? EquipmentName { get; set; }
    }
}
