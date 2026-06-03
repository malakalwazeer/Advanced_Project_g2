using System.ComponentModel.DataAnnotations;

namespace CourseManagement.ViewModels;

public class CertificationCreateViewModel
{
    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = null!;

    [MaxLength(1000)]
    public string? Description { get; set; }
}
