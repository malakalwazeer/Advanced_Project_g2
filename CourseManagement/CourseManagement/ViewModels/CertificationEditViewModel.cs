using System.ComponentModel.DataAnnotations;

namespace CourseManagement.ViewModels;

public class CertificationEditViewModel
{
    public int CertificationId { get; set; }

    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = null!;

    [MaxLength(1000)]
    public string? Description { get; set; }
}
