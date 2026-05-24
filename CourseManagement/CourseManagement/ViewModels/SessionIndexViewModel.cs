using System;
using System.ComponentModel.DataAnnotations;

namespace CourseManagement.ViewModels
{
    public class SessionIndexViewModel
    {
        public int SessionId { get; set; }

        [Display(Name = "Course")]
        public string CourseName { get; set; }

        [Display(Name = "Instructor")]
        public string InstructorName { get; set; }

        [Display(Name = "Classroom")]
        public string ClassroomLocation { get; set; }

        [Display(Name = "Start Time")]
        public DateTime StartDateTime { get; set; }

        [Display(Name = "End Time")]
        public DateTime EndDateTime { get; set; }

        public int Capacity { get; set; }
    }
}
