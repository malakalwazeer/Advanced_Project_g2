using System.ComponentModel.DataAnnotations;

namespace CourseManagement.ViewModels
{
    public class CourseReqEqCreateViewModel
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
    }
}
