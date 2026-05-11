using System;
using System.Collections.Generic;

namespace CourseManagementAPI.Models;

public partial class CoursePrerequisite
{
    //
    public int CourseId { get; set; }

    public int CoursePrerequisiteId { get; set; }

    public virtual Course Course { get; set; } = null!;

    public virtual Course PrerequisiteCourse { get; set; } = null!;
}
