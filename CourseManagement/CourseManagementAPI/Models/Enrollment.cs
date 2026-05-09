using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace CourseManagementAPI.Models;

[Keyless]
[Table("Enrollment")]
public partial class Enrollment
{
    [Column("enrollmentID")]
    public int EnrollmentId { get; set; }

    [Column("traineeID")]
    public int TraineeId { get; set; }

    [Column("sessionID")]
    public int SessionId { get; set; }

    [Column("enrollmentDate")]
    public DateOnly EnrollmentDate { get; set; }

    [Column("eStatusID")]
    public int EStatusId { get; set; }
}
