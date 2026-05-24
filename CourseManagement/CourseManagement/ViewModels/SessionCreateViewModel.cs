using System;
using System.ComponentModel.DataAnnotations;

namespace CourseManagement.ViewModels
{
    public class SessionCreateViewModel
    {
        [Required]
        [Display(Name = "Course")]
        public int CourseId { get; set; }

        [Required]
        [Display(Name = "Instructor")]
        public int InstructorId { get; set; }

        [Required]
        [Display(Name = "Classroom")]
        public int ClassroomId { get; set; }

        [Required]
        [Display(Name = "Start Date & Time")]
        [DataType(DataType.DateTime)]
        public DateTime StartDateTime { get; set; } = DateTime.Now;

        [Required]
        [Display(Name = "End Date & Time")]
        [DataType(DataType.DateTime)]
        public DateTime EndDateTime { get; set; } = DateTime.Now.AddHours(2);

        [Required]
        [Range(1, 500)]
        public int Capacity { get; set; }
    }
}
