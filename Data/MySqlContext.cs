using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using JuanJoseVargas.Models;

namespace JuanJoseVargas.Data
{
    public class MySqlContext : DbContext
    {
        public MySqlContext(DbContextOptions<MySqlContext> options) : base(options)
        {
        }

        public DbSet<Patient> Patients { get; set; }
        public DbSet<Doctor> Doctors { get; set; }
        public DbSet<Appointment> Appointments { get; set; }
        public DbSet<EmailLog> EmailLogs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
       
            modelBuilder.Entity<Patient>()
                .HasIndex(p => p.Document)
                .IsUnique();

            modelBuilder.Entity<Doctor>()
                .HasIndex(d => d.Document)
                .IsUnique();

   
            modelBuilder.Entity<Appointment>()
                .HasOne(a => a.Patient)
                .WithMany(p => p.Appointments)
                .HasForeignKey(a => a.PatientId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Appointment>()
                .HasOne(a => a.Doctor)
                .WithMany(d => d.Appointments)
                .HasForeignKey(a => a.DoctorId)
                .OnDelete(DeleteBehavior.Restrict);

            
            var statusConverter = new ValueConverter<AppointmentStatus, string>(
                v => v == AppointmentStatus.Scheduled ? "Programada" :
                     v == AppointmentStatus.Attended ? "Atendida" :
                     v == AppointmentStatus.Canceled ? "Cancelada" :
                     "Programada",
                v => v == "Programada" ? AppointmentStatus.Scheduled :
                     v == "Atendida" ? AppointmentStatus.Attended :
                     v == "Cancelada" ? AppointmentStatus.Canceled :
                     AppointmentStatus.Scheduled
            );

            modelBuilder.Entity<Appointment>()
                .Property(a => a.Status)
                .HasConversion(statusConverter)
                .HasMaxLength(50)
                .HasColumnType("varchar(50)");

            base.OnModelCreating(modelBuilder);
        }
    }
}
