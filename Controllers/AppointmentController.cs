using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using JuanJoseVargas.Data;
using JuanJoseVargas.Models;
using System.Net;
using System.Net.Mail;


namespace JuanJoseVargas.Controllers
{
    public class AppointmentController : Controller
    {
        private readonly MySqlContext _context;

        public AppointmentController(MySqlContext context)
        {
            _context = context;
        }


        [HttpGet]
        public IActionResult Create(string? document)
        {
            var doctors = _context.Doctors.ToList();
            ViewBag.Document = document;

            if (!string.IsNullOrEmpty(document))
            {
                var patient = _context.Patients.FirstOrDefault(p => p.Document == document);
                if (patient != null)
                {
                    ViewBag.PatientId = patient.Id;
                    ViewBag.PatientName = patient.FullName;
                    ViewBag.PatientEmail = patient.Email;
                }
                else
                {
                    ViewBag.Message = "No se encontró un paciente con ese documento.";
                }
            }

            return View(doctors);
        }

     
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(
            int PatientId,
            int DoctorAppointment,
            DateTime AppointmentDate,
            TimeSpan AppointmentTime,
            AppointmentStatus Status)
        {
      
            if (PatientId == 0 || DoctorAppointment == 0)
            {
                TempData["message"] = "Debe seleccionar un paciente y un médico.";
                var doctors = _context.Doctors.ToList();
                return View(doctors);
            }

            DateTime fullDateTime = AppointmentDate.Date.Add(AppointmentTime);

            
            bool doctorBusy = _context.Appointments.Any(a =>
                a.DoctorId == DoctorAppointment &&
                a.AppointmentDate == fullDateTime);

            if (doctorBusy)
            {
                TempData["message"] = "El médico ya tiene una cita en ese horario.";
                var doctors = _context.Doctors.ToList();
                return View(doctors);
            }


            var appointment = new Appointment
            {
                PatientId = PatientId,
                DoctorId = DoctorAppointment,
                AppointmentDate = fullDateTime,
                Status = Status
            };


            _context.Appointments.Add(appointment);
            _context.SaveChanges();

            TempData["message"] = "Cita creada exitosamente.";
            return RedirectToAction("Index");
        }

      
        [HttpGet]
public async Task<IActionResult> Edit(int id)
{
    var appointment = await _context.Appointments
        .Include(a => a.Patient)
        .Include(a => a.Doctor)
        .FirstOrDefaultAsync(a => a.Id == id);

    if (appointment == null)
    {
        TempData["message"] = "No se encontró la cita.";
        return RedirectToAction("Index");
    }

   
    ViewBag.Doctors = await _context.Doctors.ToListAsync();

    return View(appointment);
}

[HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> Edit(int id, Appointment updatedAppointment)
{
    if (id != updatedAppointment.Id)
    {
        return NotFound();
    }

  
    if (updatedAppointment.DoctorId == 0 || updatedAppointment.PatientId == 0)
    {
        TempData["message"] = "Debe seleccionar un paciente y un médico.";
        ViewBag.Doctors = await _context.Doctors.ToListAsync();
        return View(updatedAppointment);
    }

    try
    {
        var appointment = await _context.Appointments
            .Include(a => a.Patient)
            .Include(a => a.Doctor)
            .FirstOrDefaultAsync(a => a.Id == id);

        if (appointment == null)
        {
            TempData["message"] = "No se encontró la cita.";
            return RedirectToAction("Index");
        }

   
        bool doctorBusy = await _context.Appointments.AnyAsync(a =>
            a.DoctorId == updatedAppointment.DoctorId &&
            a.Id != id &&
            a.AppointmentDate == updatedAppointment.AppointmentDate);

        if (doctorBusy)
        {
            TempData["message"] = "El médico ya tiene una cita en ese horario.";
            ViewBag.Doctors = await _context.Doctors.ToListAsync();
            return View(updatedAppointment);
        }

       
        appointment.DoctorId = updatedAppointment.DoctorId;
        appointment.AppointmentDate = updatedAppointment.AppointmentDate;
        appointment.Status = updatedAppointment.Status;

        _context.Update(appointment);
        await _context.SaveChangesAsync();

        TempData["message"] = "✅ Cita actualizada correctamente.";
        return RedirectToAction("Index");
    }
    catch (Exception ex)
    {
        TempData["message"] = "Error al actualizar la cita: " + ex.Message;
        ViewBag.Doctors = await _context.Doctors.ToListAsync();
        return View(updatedAppointment);
    }
}



        public IActionResult Index()
        {
            var appointments = _context.Appointments
                .Include(a => a.Doctor)
                .Include(a => a.Patient)
                .Include(a => a.EmailLogs)
                .ToList();

            return View(appointments);
        }
        
        [HttpPost]
public async Task<IActionResult> SendEmail(int appointmentId)
{
    var appointment = await _context.Appointments
        .Include(a => a.Patient)
        .Include(a => a.Doctor)
        .FirstOrDefaultAsync(a => a.Id == appointmentId);

    if (appointment == null)
    {
        TempData["message"] = "No se encontró la cita para enviar el correo.";
        return RedirectToAction("Index");
    }

    if (string.IsNullOrEmpty(appointment.Patient.Email))
    {
        TempData["message"] = "El paciente no tiene un correo registrado.";
        return RedirectToAction("Index");
    }

    try
    {
        
        var smtp = new SmtpClient("smtp.gmail.com")
        {
            Port = 587,
            Credentials = new NetworkCredential("juanjohervar0817@gmail.com", "avli cvqf ccty orha"),
            EnableSsl = true
        };

        string subject = "Confirmación de cita médica";
        string body = $@"
            <h2>Hola, {appointment.Patient.FullName}</h2>
            <p>Tu cita médica ha sido programada con éxito.</p>
            <ul>
                <li><b>Doctor:</b> {appointment.Doctor.FullName}</li>
                <li><b>Especialidad:</b> {appointment.Doctor.Specialty}</li>
                <li><b>Fecha:</b> {appointment.AppointmentDate:dd/MM/yyyy HH:mm}</li>
                <li><b>Estado:</b> {appointment.Status}</li>
            </ul>
            <p>Por favor llega 10 minutos antes de tu cita.</p>
            <br><p><em>Centro Médico Juan José Vargas</em></p>";

        var mail = new MailMessage
        {
            From = new MailAddress("tucorreo@gmail.com", "Centro Médico Juan José Vargas"),
            Subject = subject,
            Body = body,
            IsBodyHtml = true
        };

        mail.To.Add(appointment.Patient.Email);

        await smtp.SendMailAsync(mail);

 
        var log = new EmailLog
        {
            AppointmentId = appointment.Id,
            Recipient = appointment.Patient.Email,
            Status = EmailStatus.Sent,
            SentDate = DateTime.Now
        };

        _context.EmailLogs.Add(log);
        await _context.SaveChangesAsync();

        TempData["message"] = "Correo enviado correctamente.";
    }
    catch (Exception ex)
    {
       
        var log = new EmailLog
        {
            AppointmentId = appointment.Id,
            Recipient = appointment.Patient.Email,
            Status = EmailStatus.NotSent,
            SentDate = DateTime.Now
        };

        _context.EmailLogs.Add(log);
        await _context.SaveChangesAsync();

        TempData["message"] = "Error al enviar el correo: " + ex.Message;
    }

    return RedirectToAction("Index");
}
    }
}
