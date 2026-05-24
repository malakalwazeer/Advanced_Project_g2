using System;
using System.ComponentModel.DataAnnotations;

namespace CourseManagement.ViewModels
{
    public class SessionEditViewModel
    {
        public int SessionId { get; set; }

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
        public DateTime StartDateTime { get; set; }

        [Required]
        [Display(Name = "End Date & Time")]
        [DataType(DataType.DateTime)]
        public DateTime EndDateTime { get; set; }

        [Required]
        [Range(1, 500)]
        public int Capacity { get; set; }
    }
}
