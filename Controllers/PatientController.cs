namespace JuanJoseVargas.Controllers;
using JuanJoseVargas.Models;
using JuanJoseVargas.Data;
using Microsoft.AspNetCore.Mvc;

public class PatientController: Controller
{
    private readonly MySqlContext _context;
    
    public PatientController(MySqlContext context)
    {
        _context = context;
    }

    public IActionResult Index()
    {
        var patients = _context.Patients.ToList();
        return View(patients);
    }

    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    public IActionResult Register([Bind("FullName,Document,Age,Phone,Email")] Patient newPatient)
    {
        if (!ModelState.IsValid)
        {
            ModelState.AddModelError("", "Por favor, ingresa correctamente los datos del paciente.");
            return View("Create", newPatient);
        }
        if (_context.Patients.Any(d => d.Document == newPatient.Document))
        {
            TempData["message"] = "Ya existe este Paciente.";
            return View("Create", newPatient);
        }
        _context.Patients.Add(newPatient);
        _context.SaveChanges();
        TempData["message"] = "Paciente registrado con éxito.";
        return RedirectToAction("Index");

    }

    public IActionResult Delete(int id)
    {
        var deletePatient = _context.Patients.Find(id);
        _context.Patients.Remove(deletePatient);
        _context.SaveChanges();
        TempData["message"] = "Paciente Eliminado con éxito.";
        return RedirectToAction("Index");
    }
    
    [HttpGet]
    public IActionResult Edit(int id)
    {
        var patient = _context.Patients.FirstOrDefault(d => d.Id == id);
        if (patient == null)
        {
            return NotFound();
        }

        return View(patient);
    }



    [HttpPost]
    public IActionResult Edit(int id, [Bind("Id,FullName,Document,Age,Phone,Email")] Patient editPatient)
    {
        var patient = _context.Patients.FirstOrDefault(d => d.Id == id);
        if (patient == null)
        {
            return NotFound();
        }

        patient.FullName = editPatient.FullName;
        patient.Document = editPatient.Document;
        patient.Age = editPatient.Age;
        patient.Phone = editPatient.Phone;
        patient.Email = editPatient.Email;

        _context.SaveChanges();

        TempData["message"] = "Datos del paciente actualizados correctamente.";
        return RedirectToAction("Index");
    }
    
}