using System;
using System.Collections.Generic;

namespace CourseManagementAPI.Models;

public partial class Notification
{
    public int NotificationId { get; set; }

    public string Title { get; set; } = null!;

    public string Message { get; set; } = null!;

    public int? SessionId { get; set; }

    public int? PaymentId { get; set; }

    public virtual Payment? Payment { get; set; }

    public virtual CourseSession? Session { get; set; }

}
