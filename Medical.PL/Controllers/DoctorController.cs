using Medical.PL.Data.Context;
using Medical.PL.Data.Models;
using Medical.PL.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;



namespace Medical.PL.Controllers
{
    public class DoctorController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;

        public DoctorController(AppDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        // GET: Doctors
        public async Task<IActionResult> Index()
        {
            var doctors = await _context.Doctors
                .Include(d => d.User)
                .Include(d => d.Department)
                .ToListAsync();
            ViewData["ActivePage"] = "Doctors";
            return View(doctors);
        }

        // GET: Doctors Details
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var doctor = await _context.Doctors
                .Include(d => d.User)
                .Include(d => d.Department)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (doctor == null) return NotFound();
            ViewData["ActivePage"] = "Doctors";
            return View(doctor);
        }

        // GET: Doctors/Create
        public IActionResult Create()
        {
            PopulateDropdowns();
            ViewData["ActivePage"] = "Doctors";
            return View(new DoctorViewModel());
        }

        // POST: Doctors Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(DoctorViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                PopulateDropdowns();
                ViewData["ActivePage"] = "Doctors";
                return View(vm);
            }

            var doctor = new Doctor
            {
                UserId = vm.UserId,
                DepartmentId = vm.DepartmentId,
                Specialization = vm.Specialization,
                ExperienceYears = vm.ExperienceYears,
                Bio = vm.Bio,
                IsActive = vm.IsActive,
                Image = await SaveImageAsync(vm.ImageFile)
            };

            _context.Add(doctor);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: Doctor Edit
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var doctor = await _context.Doctors.FindAsync(id);
            if (doctor == null) return NotFound();

            var vm = new DoctorViewModel
            {
                Id = doctor.Id,
                UserId = doctor.UserId,
                DepartmentId = doctor.DepartmentId,
                Specialization = doctor.Specialization,
                ExperienceYears = doctor.ExperienceYears,
                Bio = doctor.Bio,
                Image = doctor.Image,
                IsActive = doctor.IsActive
            };

            PopulateDropdowns();
            ViewData["ActivePage"] = "Doctors";
            return View(vm);
        }

        // POST: Doctor Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, DoctorViewModel vm)
        {
            if (id != vm.Id) return NotFound();

            if (!ModelState.IsValid)
            {
                PopulateDropdowns();
                ViewData["ActivePage"] = "Doctors";
                return View(vm);
            }

            var doctor = await _context.Doctors.FindAsync(id);
            if (doctor == null) return NotFound();

            doctor.UserId = vm.UserId;
            doctor.DepartmentId = vm.DepartmentId;
            doctor.Specialization = vm.Specialization;
            doctor.ExperienceYears = vm.ExperienceYears;
            doctor.Bio = vm.Bio;
            doctor.IsActive = vm.IsActive;

            if (vm.ImageFile != null)
                doctor.Image = await SaveImageAsync(vm.ImageFile);

            _context.Update(doctor);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: Doctor Delete
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var doctor = await _context.Doctors
                .Include(d => d.User)
                .Include(d => d.Department)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (doctor == null) return NotFound();
            ViewData["ActivePage"] = "Doctors";
            return View(doctor);
        }

        // POST: Doctors Delete
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var doctor = await _context.Doctors.FindAsync(id);
            if (doctor != null) _context.Doctors.Remove(doctor);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: Doctor/MedicalReport/5
        public async Task<IActionResult> MedicalReport(int? id)
        {
            if (id == null) return NotFound();

            var appointment = await GetAppointmentForReportAsync(id.Value);
            if (appointment == null) return NotFound();

            await PopulateMedicinesDropdownAsync();
            ViewData["ActivePage"] = "Doctors";
            return View(MapToMedicalReportViewModel(appointment));
        }

        // POST: Doctor/MedicalReport/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MedicalReport(int id, MedicalReportViewModel vm)
        {
            if (id != vm.AppointmentId) return NotFound();

            var appointment = await GetAppointmentForReportAsync(id);
            if (appointment == null) return NotFound();

            ValidatePrescriptionItems(vm.Items);

            if (!ModelState.IsValid)
            {
                await PopulateMedicinesDropdownAsync();
                ViewData["ActivePage"] = "Doctors";
                FillReportHeader(vm, appointment);
                EnsurePrescriptionRows(vm);
                return View(vm);
            }

            var prescriptionItems = vm.Items
                .Where(i => i.MedicineId.HasValue)
                .ToList();

            var prescription = appointment.Prescription;
            if (prescription == null)
            {
                prescription = new Prescription
                {
                    AppointmentId = appointment.Id,
                    DoctorId = appointment.DoctorId,
                    PatientId = appointment.PatientId
                };
                _context.Prescriptions.Add(prescription);
            }
            else
            {
                _context.PrescriptionItems.RemoveRange(prescription.Items);
            }

            prescription.Notes = vm.Notes;
            foreach (var item in prescriptionItems)
            {
                prescription.Items.Add(new PrescriptionItem
                {
                    MedicineId = item.MedicineId!.Value,
                    Dosage = item.Dosage!.Trim(),
                    Quantity = item.Quantity!.Value,
                    Duration = item.Duration!.Trim(),
                    Instructions = string.IsNullOrWhiteSpace(item.Instructions) ? null : item.Instructions.Trim()
                });
            }

            appointment.Status = "Completed";
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "تم حفظ التقرير الطبي والروشتة بنجاح";
            return RedirectToAction(nameof(MedicalReport), new { id = appointment.Id });
        }

        // Helper Methods
        private void PopulateDropdowns()
        {
            ViewBag.Users = new SelectList(_context.Users, "Id", "Name");
            ViewBag.Departments = new SelectList(_context.Departments, "Id", "Name");
        }

        private async Task PopulateMedicinesDropdownAsync()
        {
            ViewBag.Medicines = new SelectList(
                await _context.Medicines.OrderBy(m => m.Name).ToListAsync(),
                "Id",
                "Name");
        }

        private async Task<Appointment?> GetAppointmentForReportAsync(int appointmentId)
        {
            return await _context.Appointments
                .Include(a => a.Patient).ThenInclude(p => p.User)
                .Include(a => a.Doctor).ThenInclude(d => d.User)
                .Include(a => a.Service)
                .Include(a => a.Prescription!).ThenInclude(p => p.Items).ThenInclude(i => i.Medicine)
                .FirstOrDefaultAsync(a => a.Id == appointmentId);
        }

        private static MedicalReportViewModel MapToMedicalReportViewModel(Appointment appointment)
        {
            var vm = new MedicalReportViewModel
            {
                AppointmentId = appointment.Id,
                DoctorId = appointment.DoctorId,
                PatientId = appointment.PatientId,
                Notes = appointment.Prescription?.Notes
            };

            FillReportHeader(vm, appointment);

            if (appointment.Prescription != null)
            {
                vm.Items = appointment.Prescription.Items
                    .Select(i => new MedicalReportPrescriptionItemViewModel
                    {
                        MedicineId = i.MedicineId,
                        Dosage = i.Dosage,
                        Quantity = i.Quantity,
                        Duration = i.Duration,
                        Instructions = i.Instructions
                    })
                    .ToList();
            }

            EnsurePrescriptionRows(vm);
            return vm;
        }

        private static void FillReportHeader(MedicalReportViewModel vm, Appointment appointment)
        {
            vm.AppointmentId = appointment.Id;
            vm.DoctorId = appointment.DoctorId;
            vm.PatientId = appointment.PatientId;
            vm.PatientName = appointment.Patient?.User?.Name;
            vm.PatientPhone = appointment.Patient?.User?.PhoneNumber;
            vm.DoctorName = appointment.Doctor?.User?.Name;
            vm.DoctorSpecialization = appointment.Doctor?.Specialization;
            vm.ServiceName = appointment.Service?.Name;
            vm.AppointmentDate = appointment.AppointmentDate;
            vm.AppointmentTime = appointment.AppointmentTime;
        }

        private static void EnsurePrescriptionRows(MedicalReportViewModel vm)
        {
            while (vm.Items.Count < 5)
            {
                vm.Items.Add(new MedicalReportPrescriptionItemViewModel());
            }
        }

        private void ValidatePrescriptionItems(List<MedicalReportPrescriptionItemViewModel> items)
        {
            var completedRowsCount = 0;

            for (var i = 0; i < items.Count; i++)
            {
                var hasAnyValue = items[i].MedicineId.HasValue
                    || !string.IsNullOrWhiteSpace(items[i].Dosage)
                    || items[i].Quantity.HasValue
                    || !string.IsNullOrWhiteSpace(items[i].Duration)
                    || !string.IsNullOrWhiteSpace(items[i].Instructions);

                if (!hasAnyValue)
                    continue;

                if (!items[i].MedicineId.HasValue)
                    ModelState.AddModelError($"Items[{i}].MedicineId", "الدواء مطلوب.");

                if (string.IsNullOrWhiteSpace(items[i].Dosage))
                    ModelState.AddModelError($"Items[{i}].Dosage", "الجرعة مطلوبة.");

                if (items[i].Quantity == null || items[i].Quantity <= 0)
                    ModelState.AddModelError($"Items[{i}].Quantity", "الكمية مطلوبة ويجب أن تكون أكبر من صفر.");

                if (string.IsNullOrWhiteSpace(items[i].Duration))
                    ModelState.AddModelError($"Items[{i}].Duration", "المدة مطلوبة.");

                if (items[i].MedicineId.HasValue
                    && !string.IsNullOrWhiteSpace(items[i].Dosage)
                    && items[i].Quantity > 0
                    && !string.IsNullOrWhiteSpace(items[i].Duration))
                {
                    completedRowsCount++;
                }
            }

            if (completedRowsCount == 0)
                ModelState.AddModelError(nameof(MedicalReportViewModel.Items), "يجب إضافة دواء واحد على الأقل في الروشتة.");
        }

        private async Task<string?> SaveImageAsync(IFormFile? file)
        {
            if (file == null || file.Length == 0) return null;

            var uploadsFolder = Path.Combine(_env.WebRootPath, "uploads", "doctors");
            Directory.CreateDirectory(uploadsFolder);

            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
            var filePath = Path.Combine(uploadsFolder, fileName);

            using var stream = new FileStream(filePath, FileMode.Create);
            await file.CopyToAsync(stream);

            return $"/uploads/doctors/{fileName}";
        }
       
        }
}
