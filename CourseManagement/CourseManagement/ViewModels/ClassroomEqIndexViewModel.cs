namespace CourseManagement.ViewModels;

public class ClassroomEqIndexViewModel
{
    public int ClassroomId { get; set; }
    public int EquipmentId { get; set; }
    public string LocationName { get; set; } = string.Empty;
    public string EquipmentName { get; set; } = string.Empty;
    public int Quantity { get; set; }
}