using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace CourseManagementAPI.Models;

[Keyless]
[Table("CourseReqEquipment")]
public partial class CourseReqEquipment
{
    [Column("equipmentID")]
    public int EquipmentId { get; set; }

    [Column("courseID")]
    public int CourseId { get; set; }

    [Column("quantity")]
    public int Quantity { get; set; }
}
