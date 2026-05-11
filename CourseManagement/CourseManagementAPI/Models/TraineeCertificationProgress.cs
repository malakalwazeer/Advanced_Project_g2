using System;
using System.Collections.Generic;

namespace CourseManagementAPI.Models;
public partial class TraineeCertificationProgress
{
    //
    public int TraineeId { get; set; }

    public int CertificationId { get; set; }

    public DateOnly? AchievedDate { get; set; }

    public decimal ProgressPercentage { get; set; }

    public virtual Trainee Trainee { get; set; } = null!;

    public virtual Certification Certification { get; set; } = null!;
}
