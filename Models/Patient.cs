namespace JuanJoseVargas.Models;
using System.ComponentModel.DataAnnotations;

public class Patient
{
    public int Id { get; set; }

    [Required]
    [MaxLength(100)]
    public string FullName { get; set; }

    [Required]
    [MaxLength(20)]
    public string Document { get; set; }

    [Range(0, 120)]
    public int Age { get; set; }

    [MaxLength(15)]
    public string Phone { get; set; }

    [EmailAddress]
    public string Email { get; set; }

    // Navigation property
    public ICollection<Appointment>? Appointments { get; set; }
}