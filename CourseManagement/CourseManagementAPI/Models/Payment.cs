using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace CourseManagementAPI.Models;

[Keyless]
[Table("Payment")]
public partial class Payment
{
    [Column("paymentID")]
    public int PaymentId { get; set; }

    [Column("enrollmentID")]
    public int EnrollmentId { get; set; }

    [Column("amountPaid", TypeName = "decimal(18, 0)")]
    public decimal AmountPaid { get; set; }

    [Column("paymentDate")]
    public DateOnly PaymentDate { get; set; }

    [Column("pStatusID")]
    public int PStatusId { get; set; }

    [Column("balanceRemaining", TypeName = "decimal(18, 0)")]
    public decimal? BalanceRemaining { get; set; }
}
