using System;
using System.Collections.Generic;

namespace CourseManagementAPI.Models;

public partial class Classroom
{
    public int ClassroomId { get; set; }

    public string Location { get; set; } = null!;

    public int Capacity { get; set; } 

    public bool IsActive { get; set; }

    public virtual ICollection<CourseSession> CourseSessions { get; set; } = new List<CourseSession>();

    //
    public virtual ICollection<ClassroomEquipment> ClassroomEquipments { get; set; } = new List<ClassroomEquipment>();
}
