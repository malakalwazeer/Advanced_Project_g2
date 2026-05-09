using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace CourseManagementAPI.Models;

[Keyless]
[Table("TraineeCertificationProgress")]
public partial class TraineeCertificationProgress
{
    [Column("traineeID")]
    public int TraineeId { get; set; }

    [Column("certificationID")]
    public int CertificationId { get; set; }

    [Column("achievedDate")]
    public DateOnly? AchievedDate { get; set; }

    [Column("progressPercentage", TypeName = "decimal(18, 0)")]
    public decimal ProgressPercentage { get; set; }
}
