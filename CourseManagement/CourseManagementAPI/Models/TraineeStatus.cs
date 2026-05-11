using System;
using System.Collections.Generic;

namespace CourseManagementAPI.Models;

public partial class TraineeStatus
{
    public int TraineeStatusId { get; set; }

    public string StatusName { get; set; } = null!;

    public virtual ICollection<Trainee> Trainees { get; set; } = new List<Trainee>();
}
