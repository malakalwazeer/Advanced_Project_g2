using System.ComponentModel.DataAnnotations;

namespace CourseManagement.ViewModels;

public class ClassroomEqEditViewModel
{
    public int ClassroomId { get; set; }
    public int EquipmentId { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Quantity must be greater than zero.")]
    public int Quantity { get; set; }
}
