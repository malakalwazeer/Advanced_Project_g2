using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace CourseManagementAPI.Models;

[Keyless]
[Table("PaymentStatus")]
public partial class PaymentStatus
{
    [Column("pStatusID")]
    public int PStatusId { get; set; }

    [Column("statusName")]
    public string StatusName { get; set; } = null!;
}
