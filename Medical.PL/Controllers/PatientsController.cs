using Medical.PL.Services;
using Medical.PL.ViewModels;
using Medical.PL.Data.Context;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Medical.PL.Controllers
{
    [Authorize]
    public class PatientsController : Controller
    {
        private readonly IPatientService _patientService;
        private readonly AppDbContext _context;

public PatientsController(IPatientService patientService, AppDbContext context)
{
    _patientService = patientService;
    _context = context;
}


        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index()
        {
            var patients = await _patientService.GetAllAsync();
            return View(patients);
        }

        public async Task<IActionResult> Details(int id)
        {
            var patient = await _patientService.GetByIdAsync(id);

            if (patient == null)
                return NotFound();

            return View(patient);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PatientVM vm)
        {
            if (!ModelState.IsValid)
                return View(vm);

            await _patientService.CreateAsync(vm);

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var vm = await _patientService.GetForEditAsync(id);

            if (vm == null)
                return NotFound();

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, PatientVM vm)
        {
            if (!ModelState.IsValid)
                return View(vm);

            var success = await _patientService.UpdateAsync(id, vm);

            if (!success)
                return NotFound();

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int id)
        {
            var patient = await _patientService.GetByIdAsync(id);

            if (patient == null)
                return NotFound();

            return View(patient);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var success = await _patientService.DeleteAsync(id);

            if (!success)
                return NotFound();

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Appointments(int id)
        {
            var appointments = await _patientService.GetAppointmentsAsync(id);

            if (appointments == null)
                return NotFound();

            return View(appointments);
        }

        public async Task<IActionResult> Prescriptions(int id)
        {
            var (prescriptions, patientName, patientId) = await _patientService.GetPrescriptionsAsync(id);

            if (prescriptions == null)
                return NotFound();

            ViewBag.PatientName = patientName;
            ViewBag.PatientId = patientId;

            return View(prescriptions);
        }

        public async Task<IActionResult> PrescriptionDetails(int id)
        {
            // show a single prescription with limited fields for patient view and print option
            var prescription = await _context.Prescriptions
                .Include(p => p.Doctor).ThenInclude(d => d.User)
                .Include(p => p.Doctor).ThenInclude(d => d.Department)
                .Include(p => p.Patient).ThenInclude(pt => pt.User)
                .Include(p => p.Appointment).ThenInclude(a => a.Service)
                .Include(p => p.Items).ThenInclude(i => i.Medicine)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (prescription == null)
                return NotFound();

            // ensure only the owner patient or admin/doctor can view (basic check)
            var userId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier).Value);
            // allow if current user is the patient owner or in Admin role or is the doctor who wrote it
            if (prescription.Patient?.User != null && prescription.Patient.User.Id != userId && !User.IsInRole("Admin") && prescription.Doctor?.User?.Id != userId)
            {
                return Forbid();
            }

            return View(prescription);
        }
    }
}
