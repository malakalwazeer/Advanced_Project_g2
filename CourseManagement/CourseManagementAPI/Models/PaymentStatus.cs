using System;
using System.Collections.Generic;

namespace CourseManagementAPI.Models;

public partial class PaymentStatus
{
    public int PaymentStatusId { get; set; }

    public string StatusName { get; set; } = null!;

    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();
}
