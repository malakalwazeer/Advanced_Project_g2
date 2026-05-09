using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace CourseManagementAPI.Models;

[Keyless]
[Table("Assessment")]
public partial class Assessment
{
    [Column("assessmentID")]
    public int AssessmentId { get; set; }

    [Column("enrollmentID")]
    public int EnrollmentId { get; set; }

    [Column("instructorID")]
    public int InstructorId { get; set; }

    [Column("result")]
    public int? Result { get; set; }

    [Column("score")]
    public int? Score { get; set; }
}
