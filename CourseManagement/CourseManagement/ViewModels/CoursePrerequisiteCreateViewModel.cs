using System.ComponentModel.DataAnnotations;

namespace CourseManagement.ViewModels
{
    public class CoursePrerequisiteCreateViewModel
    {
        [Required]
        [Display(Name = "Select Course")]
        public int CourseId { get; set; }

        [Required]
        [Display(Name = "Select Prerequisite")]
        public int PrerequisiteCourseId { get; set; }
    }
}
