namespace JuanJoseVargas.Models;
using System.ComponentModel.DataAnnotations;

public enum EmailStatus
{
    Sent,
    NotSent
}

public class EmailLog
{
    public int Id { get; set; }

    [Required]
    public int AppointmentId { get; set; }
    public Appointment Appointment { get; set; }

    [Required]
    [EmailAddress]
    public string Recipient { get; set; }

    [Required]
    public EmailStatus Status { get; set; }

    public DateTime SentDate { get; set; } = DateTime.Now;
}