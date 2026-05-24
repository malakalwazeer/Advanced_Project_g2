using System.ComponentModel.DataAnnotations;

namespace CourseManagement.ViewModels
{
    public class CoursePrerequisiteIndexViewModel
    {
        public int CourseId { get; set; }
        public int PrerequisiteCourseId { get; set; }

        [Display(Name = "Course Name")]
        public string CourseName { get; set; }

        [Display(Name = "Prerequisite Course")]
        public string PrerequisiteCourseName { get; set; }
    }
}
