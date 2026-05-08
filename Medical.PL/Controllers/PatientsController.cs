using Medical.PL.Services;
using Medical.PL.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace Medical.PL.Controllers
{
    public class PatientsController : Controller
    {
        private readonly IPatientService _patientService;

        public PatientsController(IPatientService patientService)
        {
            _patientService = patientService;
        }

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
    }
}
