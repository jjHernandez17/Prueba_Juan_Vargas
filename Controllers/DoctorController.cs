namespace JuanJoseVargas.Controllers;
using JuanJoseVargas.Models;
using JuanJoseVargas.Data;
using Microsoft.AspNetCore.Mvc;


public class DoctorController : Controller
{
    private readonly MySqlContext _context;
    
    public DoctorController(MySqlContext context)
    {
        _context = context;
    }

    public IActionResult Index()
    {
        var doctors = _context.Doctors.ToList();
        return View(doctors);
    }

    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    public IActionResult Register([Bind("FullName,Document,Specialty,Phone,Email")] Doctor newDoctor)
    {
        if (!ModelState.IsValid)
        {
            ModelState.AddModelError("", "Por favor, ingresa correctamente los datos del doctor.");
            return View("Create", newDoctor);
        }

        if (_context.Doctors.Any(d => d.Document == newDoctor.Document))
        {
            TempData["message"] = "Ya existe este doctor.";
            return View("Create", newDoctor);
        }

        _context.Doctors.Add(newDoctor);
        _context.SaveChanges();

        TempData["message"] = "Doctor registrado con éxito.";

        
        return RedirectToAction("Index");
    }

    public IActionResult delete(int id)
    {
        var docDelete = _context.Doctors.Find(id);
        _context.Doctors.Remove(docDelete);
        _context.SaveChanges();
        return RedirectToAction("Index");
        
    }

    [HttpGet]
    public IActionResult Edit(int id)
    {
        var doctor = _context.Doctors.FirstOrDefault(d => d.Id == id);
        if (doctor == null)
        {
            return NotFound();
        }

        return View(doctor);
    }



    [HttpPost]
    public IActionResult Edit(int id, [Bind("Id,FullName,Document,Specialty,Phone,Email")] Doctor editDoctor)
    {
        var doctor = _context.Doctors.FirstOrDefault(d => d.Id == id);
        if (doctor == null)
        {
            return NotFound();
        }

        doctor.FullName = editDoctor.FullName;
        doctor.Document = editDoctor.Document;
        doctor.Specialty = editDoctor.Specialty;
        doctor.Phone = editDoctor.Phone;
        doctor.Email = editDoctor.Email;

        _context.SaveChanges();

        TempData["message"] = "Datos del doctor actualizados correctamente.";
        return RedirectToAction("Index");
    }
}