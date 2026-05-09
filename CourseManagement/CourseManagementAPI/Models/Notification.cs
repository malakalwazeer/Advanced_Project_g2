using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace CourseManagementAPI.Models;

[Keyless]
[Table("Notification")]
public partial class Notification
{
    [Column("notificationID")]
    public int NotificationId { get; set; }

    [Column("title")]
    public string Title { get; set; } = null!;

    [Column("message")]
    public string Message { get; set; } = null!;

    [Column("sessionID")]
    public int? SessionId { get; set; }

    [Column("paymentID")]
    public int? PaymentId { get; set; }
}
