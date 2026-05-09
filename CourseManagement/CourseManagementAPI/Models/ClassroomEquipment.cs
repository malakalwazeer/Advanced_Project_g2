using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace CourseManagementAPI.Models;

[Keyless]
[Table("ClassroomEquipment")]
public partial class ClassroomEquipment
{
    [Column("classroomID")]
    public int ClassroomId { get; set; }

    [Column("equipmentID")]
    public int EquipmentId { get; set; }

    [Column("quantity")]
    public int Quantity { get; set; }
}
