using System.ComponentModel.DataAnnotations;

namespace CourseManagement.ViewModels
{
    public class EquipmentIndexViewModel
    {
        public int EquipmentId { get; set; }

        [Display(Name = "Equipment Name")]
        public string EquipmentName { get; set; }

        public string Description { get; set; }
    }
}
