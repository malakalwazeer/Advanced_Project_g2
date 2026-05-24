using System.ComponentModel.DataAnnotations;

namespace CourseManagement.ViewModels;

public class ClassroomEqCreateViewModel
{
    [Range(1, int.MaxValue, ErrorMessage = "Please select a classroom.")]
    public int ClassroomId { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Please select equipment.")]
    public int EquipmentId { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Quantity must be greater than zero.")]
    public int Quantity { get; set; }
}
