using System;
using System.Collections.Generic;

namespace CourseManagementAPI.Models;

public partial class Payment
{
    public int PaymentId { get; set; }

    public int EnrollmentId { get; set; }

    public decimal AmountPaid { get; set; }

    public DateOnly PaymentDate { get; set; }

    public int PaymentStatusId { get; set; }

    public decimal? BalanceRemaining { get; set; }

    public virtual Enrollment Enrollment { get; set; } = null!;

    public virtual ICollection<Notification> Notifications { get; set; } = new List<Notification>();

    public virtual PaymentStatus PaymentStatus { get; set; } = null!;
}
