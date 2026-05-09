using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace CourseManagementAPI.Models;

[Keyless]
[Table("Course")]
public partial class Course
{
    [Column("courseID")]
    public int CourseId { get; set; }

    [Column("courseCode")]
    public string CourseCode { get; set; } = null!;

    [Column("courseName")]
    public string CourseName { get; set; } = null!;

    [Column("description")]
    public string? Description { get; set; }

    [Column("capacity")]
    public int Capacity { get; set; }

    [Column("enrollmentFee", TypeName = "decimal(18, 0)")]
    public decimal EnrollmentFee { get; set; }

    [Column("categoryID")]
    public int CategoryId { get; set; }
}
