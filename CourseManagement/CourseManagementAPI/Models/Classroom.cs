using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace CourseManagementAPI.Models;

[Keyless]
[Table("Classroom")]
public partial class Classroom
{
    [Column("classroomID")]
    public int ClassroomId { get; set; }

    [Column("location")]
    public string Location { get; set; } = null!;

    [Column("capacity")]
    public string Capacity { get; set; } = null!;

    [Column("isActive")]
    public bool IsActive { get; set; }
}
