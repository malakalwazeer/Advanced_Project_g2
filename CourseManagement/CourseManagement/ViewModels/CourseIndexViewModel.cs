using System.ComponentModel.DataAnnotations;

namespace CourseManagement.ViewModels
{
    public class CourseIndexViewModel
    {
        public int CourseId { get; set; }

        [Display(Name = "Code")]
        public string CourseCode { get; set; }

        public string Title { get; set; }

        [Display(Name = "Category")]
        public string CategoryName { get; set; }

        public decimal Fee { get; set; }
    }
}
