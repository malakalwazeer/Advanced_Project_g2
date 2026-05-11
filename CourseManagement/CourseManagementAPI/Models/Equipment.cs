using System;
using System.Collections.Generic;

namespace CourseManagementAPI.Models;

public partial class Equipment
{
    public int EquipmentId { get; set; }

    public string EquipmentName { get; set; } = null!;

    public string? Description { get; set; }

    //
    public virtual ICollection<ClassroomEquipment> ClassroomEquipments { get; set; } = new List<ClassroomEquipment>();

    public virtual ICollection<CourseReqEquipment> CourseReqEquipments { get; set; } = new List<CourseReqEquipment>();
}
