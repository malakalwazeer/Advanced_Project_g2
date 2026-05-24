using System.ComponentModel.DataAnnotations;

namespace CourseManagement.ViewModels
{
    public class EquipmentEditViewModel
    {
        public int EquipmentId { get; set; }

        [Required]
        [Display(Name = "Equipment Name")]
        public string EquipmentName { get; set; }

        public string Description { get; set; }
    }
}
