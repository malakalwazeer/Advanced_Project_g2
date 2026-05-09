using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace CourseManagementAPI.Models;

[Keyless]
[Table("Instructor")]
public partial class Instructor
{
    [Column("instructorID")]
    public int InstructorId { get; set; }

    [Column("fullName")]
    public string FullName { get; set; } = null!;

    [Column("email")]
    [StringLength(10)]
    public string Email { get; set; } = null!;

    [Column("phone")]
    public int Phone { get; set; }

    [Column("password")]
    public string Password { get; set; } = null!;

    [Column("qualifications")]
    public string? Qualifications { get; set; }

    [Column("hireDate", TypeName = "datetime")]
    public DateTime HireDate { get; set; }
}
