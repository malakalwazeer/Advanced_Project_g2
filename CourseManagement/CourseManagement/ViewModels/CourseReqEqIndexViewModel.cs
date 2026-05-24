using System.ComponentModel.DataAnnotations;

namespace CourseManagement.ViewModels
{
    public class CourseReqEqIndexViewModel
    {
        public int CourseId { get; set; }
        public int EquipmentId { get; set; }

        [Display(Name = "Course Name")]
        public string CourseName { get; set; }

        [Display(Name = "Required Equipment")]
        public string EquipmentName { get; set; }

        [Display(Name = "Quantity Required")]
        public int Quantity { get; set; }
    }
}
