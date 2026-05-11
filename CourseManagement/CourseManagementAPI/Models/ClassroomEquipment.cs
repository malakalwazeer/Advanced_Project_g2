using System;
using System.Collections.Generic;

namespace CourseManagementAPI.Models;

public partial class ClassroomEquipment
{
    //
    public int ClassroomId { get; set; }

    public int EquipmentId { get; set; }

    public int Quantity { get; set; }

    public virtual Classroom Classroom { get; set; } = null!;

    public virtual Equipment Equipment { get; set; } = null!;
}