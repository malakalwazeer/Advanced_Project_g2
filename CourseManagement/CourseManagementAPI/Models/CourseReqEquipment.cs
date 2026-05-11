using System;
using System.Collections.Generic;

namespace CourseManagementAPI.Models;

public partial class CourseReqEquipment
{
    //
    public int EquipmentId { get; set; }

    public int CourseId { get; set; }

    public int Quantity { get; set; }

    public virtual Equipment Equipment { get; set; } = null!;

    public virtual Course Course { get; set; } = null!;
}
