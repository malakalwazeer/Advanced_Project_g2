using System.ComponentModel.DataAnnotations;

namespace CourseManagement.ViewModels
{
    public class EquipmentCreateViewModel
    {
        [Required]
        [Display(Name = "Equipment Name")]
        public string EquipmentName { get; set; }

        public string Description { get; set; }
    }
}
