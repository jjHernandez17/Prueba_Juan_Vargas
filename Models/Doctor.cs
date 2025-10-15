namespace JuanJoseVargas.Models;
using System.ComponentModel.DataAnnotations;

public class Doctor
{
    
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string FullName { get; set; }

        [Required]
        [MaxLength(20)]
        public string Document { get; set; }

        [Required]
        [MaxLength(50)]
        public string Specialty { get; set; }

        [MaxLength(15)]
        public string Phone { get; set; }

        [EmailAddress]
        public string Email { get; set; }

       
        public ICollection<Appointment>? Appointments { get; set; }
    
}