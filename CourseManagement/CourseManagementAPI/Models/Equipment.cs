using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace CourseManagementAPI.Models;

[Keyless]
public partial class Equipment
{
    [Column("equipmentID")]
    public int EquipmentId { get; set; }

    [Column("equipmentName")]
    public string EquipmentName { get; set; } = null!;

    [Column("description")]
    public string? Description { get; set; }
}
