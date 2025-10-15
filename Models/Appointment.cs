namespace JuanJoseVargas.Models;
using System.ComponentModel.DataAnnotations;

public enum AppointmentStatus
{
    Scheduled,
    Attended,
    Canceled
}

public class Appointment
{
    public int Id { get; set; }

    [Required]
    public int PatientId { get; set; }
    public Patient Patient { get; set; }

    [Required]
    public int DoctorId { get; set; }
    public Doctor Doctor { get; set; }

    [Required]
    public DateTime AppointmentDate { get; set; }

    [Required]
    public AppointmentStatus Status { get; set; } = AppointmentStatus.Scheduled;


    public ICollection<EmailLog>? EmailLogs { get; set; }
}