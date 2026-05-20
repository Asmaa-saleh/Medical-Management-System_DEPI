using Medical.PL.Data.Context;
using Medical.PL.Data.Models;
using Medical.PL.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Medical.PL.Controllers
{

    [Authorize]
    public class DoctorController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole<int>> _roleManager;

        public DoctorController(
            AppDbContext context,
            IWebHostEnvironment env,
            UserManager<User> userManager,
            RoleManager<IdentityRole<int>> roleManager)
        {
            _context = context;
            _env = env;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        [Authorize(Roles = "Doctor")]
        public async Task<IActionResult> MyProfile()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("SignIn", "Account");

            var doctor = await _context.Doctors
                .Include(d => d.User)
                .Include(d => d.Department)
                .FirstOrDefaultAsync(d => d.UserId == user.Id);

            if (doctor == null) return NotFound();

            var appointments = await _context.Appointments
                .Include(a => a.Patient).ThenInclude(p => p.User)
                .Include(a => a.Service)
                .Include(a => a.Prescription)
                .Where(a => a.DoctorId == doctor.Id)
                .OrderBy(a => a.AppointmentDate).ThenBy(a => a.AppointmentTime)
                .ToListAsync();

            ViewBag.Appointments = appointments;
            ViewData["ActivePage"] = "DoctorProfile";
            return View(doctor);
        }

        public async Task<IActionResult> Index()
        {
            var doctors = await _context.Doctors
                .Include(d => d.User)
                .Include(d => d.Department)
                .OrderBy(d => d.User.Name)
                .ToListAsync();

            ViewData["ActivePage"] = "Doctors";
            return View(doctors);
        }

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


        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            PopulateDropdowns();
            ViewData["ActivePage"] = "Doctors";

            return View(new DoctorViewModel
            {
                DateOfBirth = DateTime.Today.AddYears(-30),
                IsActive = true
            });
        }


        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(DoctorViewModel vm)
        {
            ValidateCreatePassword(vm);

            if (!ModelState.IsValid)
            {
                PopulateDropdowns();
                ViewData["ActivePage"] = "Doctors";
                return View(vm);
            }

            await using var transaction = await _context.Database.BeginTransactionAsync();

            var user = new User
            {
                Name = vm.Name.Trim(),
                UserName = vm.Email.Trim(),
                Email = vm.Email.Trim(),
                PhoneNumber = vm.PhoneNumber.Trim(),
                DateOfBirth = vm.DateOfBirth,
                Gender = vm.Gender,
                CreatedAt = DateTime.UtcNow
            };

            var userResult = await _userManager.CreateAsync(user, vm.Password!);
            if (!userResult.Succeeded)
            {
                AddIdentityErrors(userResult);
                PopulateDropdowns();
                ViewData["ActivePage"] = "Doctors";
                return View(vm);
            }

            await AddDoctorRoleIfExistsAsync(user);

            var doctor = new Doctor
            {
                UserId = user.Id,
                DepartmentId = vm.DepartmentId,
                Specialization = vm.Specialization.Trim(),
                ExperienceYears = vm.ExperienceYears,
                Bio = string.IsNullOrWhiteSpace(vm.Bio) ? null : vm.Bio.Trim(),
                IsActive = vm.IsActive,
                Image = await SaveImageAsync(vm.ImageFile)
            };

            _context.Doctors.Add(doctor);
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            return RedirectToAction(nameof(Index));
        }


        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var doctor = await _context.Doctors
                .Include(d => d.User)
                .FirstOrDefaultAsync(d => d.Id == id);

            if (doctor == null) return NotFound();

            var vm = new DoctorViewModel
            {
                Id = doctor.Id,
                UserId = doctor.UserId,
                Name = doctor.User.Name,
                Email = doctor.User.Email ?? string.Empty,
                PhoneNumber = doctor.User.PhoneNumber ?? string.Empty,
                DateOfBirth = doctor.User.DateOfBirth,
                Gender = doctor.User.Gender,
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


        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, DoctorViewModel vm)
        {
            if (id != vm.Id) return NotFound();

            ModelState.Remove(nameof(DoctorViewModel.Password));
            ModelState.Remove(nameof(DoctorViewModel.ConfirmPassword));

            if (!ModelState.IsValid)
            {
                PopulateDropdowns();
                ViewData["ActivePage"] = "Doctors";
                return View(vm);
            }

            var doctor = await _context.Doctors
                .Include(d => d.User)
                .FirstOrDefaultAsync(d => d.Id == id);

            if (doctor == null) return NotFound();

            doctor.User.Name = vm.Name.Trim();
            doctor.User.UserName = vm.Email.Trim();
            doctor.User.Email = vm.Email.Trim();
            doctor.User.PhoneNumber = vm.PhoneNumber.Trim();
            doctor.User.DateOfBirth = vm.DateOfBirth;
            doctor.User.Gender = vm.Gender;

            var updateUserResult = await _userManager.UpdateAsync(doctor.User);
            if (!updateUserResult.Succeeded)
            {
                AddIdentityErrors(updateUserResult);
                PopulateDropdowns();
                ViewData["ActivePage"] = "Doctors";
                return View(vm);
            }

            doctor.DepartmentId = vm.DepartmentId;
            doctor.Specialization = vm.Specialization.Trim();
            doctor.ExperienceYears = vm.ExperienceYears;
            doctor.Bio = string.IsNullOrWhiteSpace(vm.Bio) ? null : vm.Bio.Trim();
            doctor.IsActive = vm.IsActive;

            if (vm.ImageFile != null)
            {
                doctor.Image = await SaveImageAsync(vm.ImageFile);
            }

            _context.Update(doctor);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }


        [Authorize(Roles = "Admin")]
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


        [Authorize(Roles = "Admin")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var doctor = await _context.Doctors.FindAsync(id);
            if (doctor != null)
            {
                _context.Doctors.Remove(doctor);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> WritePrescription(int? id)
        {
            if (id == null) return NotFound();

            var doctor = await _context.Doctors
                .Include(d => d.User)
                .FirstOrDefaultAsync(d => d.Id == id);

            if (doctor == null) return NotFound();

            if (!doctor.IsActive)
            {
                TempData["ErrorMessage"] = "لا يمكن إصدار وصفة طبية من طبيب غير نشط.";
                return RedirectToAction(nameof(Details), new { id });
            }

            await PopulateBookedAppointmentsDropdownAsync(id.Value);
            await PopulateMedicinesDropdownAsync();
            ViewData["ActivePage"] = "Doctors";

            return View(new WritePrescriptionViewModel
            {
                DoctorId = doctor.Id,
                DoctorName = doctor.User?.Name,
                Items = new List<MedicalReportPrescriptionItemViewModel> { new() }
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> WritePrescription(int id, WritePrescriptionViewModel vm)
        {
            if (id != vm.DoctorId) return NotFound();

            var doctor = await _context.Doctors
                .Include(d => d.User)
                .FirstOrDefaultAsync(d => d.Id == id);

            if (doctor == null) return NotFound();

            if (!doctor.IsActive)
            {
                TempData["ErrorMessage"] = "لا يمكن إصدار وصفة طبية من طبيب غير نشط.";
                return RedirectToAction(nameof(Details), new { id });
            }

            var appointment = await _context.Appointments
                .Include(a => a.Patient)
                .FirstOrDefaultAsync(a => a.Id == vm.AppointmentId
                                       && a.DoctorId == id
                                       && a.Status == "Booked");

            if (appointment == null)
                ModelState.AddModelError(nameof(vm.AppointmentId), "الحجز غير صالح أو لم يعد متاحاً.");

            ValidatePrescriptionItems(vm.Items);

            if (!ModelState.IsValid)
            {
                await PopulateBookedAppointmentsDropdownAsync(id);
                await PopulateMedicinesDropdownAsync();
                vm.DoctorName = doctor.User?.Name;
                if (vm.Items.Count == 0) vm.Items.Add(new());
                ViewData["ActivePage"] = "Doctors";
                return View(vm);
            }

            var prescription = new Prescription
            {
                AppointmentId = appointment!.Id,
                DoctorId = id,
                PatientId = appointment.PatientId,
                Notes = vm.Notes
            };

            foreach (var item in vm.Items.Where(i => i.MedicineId.HasValue))
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
            _context.Prescriptions.Add(prescription);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "تم حفظ الوصفة الطبية";
            return RedirectToAction(nameof(Prescriptions), new { id = vm.DoctorId });
        }

        public async Task<IActionResult> Prescriptions(int? id)
        {
            if (id == null) return NotFound();

            var doctor = await _context.Doctors
                .Include(d => d.User)
                .FirstOrDefaultAsync(d => d.Id == id);

            if (doctor == null) return NotFound();

            var prescriptions = await _context.Prescriptions
                .Include(p => p.Appointment)
                .Include(p => p.Patient).ThenInclude(p => p.User)
                .Include(p => p.Items).ThenInclude(i => i.Medicine)
                .Where(p => p.DoctorId == id)
                .OrderBy(p => p.Appointment.AppointmentDate)
                .ToListAsync();

            ViewBag.DoctorName = doctor.User?.Name;
            ViewBag.DoctorId = doctor.Id;
            ViewBag.DoctorIsActive = doctor.IsActive;
            ViewData["ActivePage"] = "Doctors";
            return View(prescriptions);
        }

        public async Task<IActionResult> EditPrescription(int? id)
        {
            if (id == null) return NotFound();

            var prescription = await _context.Prescriptions
                .Include(p => p.Doctor).ThenInclude(d => d.User)
                .Include(p => p.Patient).ThenInclude(p => p.User)
                .Include(p => p.Appointment).ThenInclude(a => a!.Service)
                .Include(p => p.Items).ThenInclude(i => i.Medicine)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (prescription == null) return NotFound();

            await PopulateMedicinesDropdownAsync();
            ViewData["ActivePage"] = "Doctors";

            var vm = new EditPrescriptionViewModel
            {
                PrescriptionId = prescription.Id,
                DoctorId = prescription.DoctorId,
                DoctorName = prescription.Doctor?.User?.Name,
                PatientName = prescription.Patient?.User?.Name,
                AppointmentDate = prescription.Appointment?.AppointmentDate,
                AppointmentTime = prescription.Appointment?.AppointmentTime,
                ServiceName = prescription.Appointment?.Service?.Name,
                Notes = prescription.Notes,
                Items = prescription.Items.Select(i => new MedicalReportPrescriptionItemViewModel
                {
                    MedicineId = i.MedicineId,
                    Dosage = i.Dosage,
                    Quantity = i.Quantity,
                    Duration = i.Duration,
                    Instructions = i.Instructions
                }).ToList()
            };

            if (vm.Items.Count == 0)
                vm.Items.Add(new MedicalReportPrescriptionItemViewModel());

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditPrescription(int id, EditPrescriptionViewModel vm)
        {
            if (id != vm.PrescriptionId) return NotFound();

            var prescription = await _context.Prescriptions
                .Include(p => p.Items)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (prescription == null) return NotFound();

            ValidatePrescriptionItems(vm.Items);

            if (!ModelState.IsValid)
            {
                var full = await _context.Prescriptions
                    .Include(p => p.Doctor).ThenInclude(d => d.User)
                    .Include(p => p.Patient).ThenInclude(p => p.User)
                    .Include(p => p.Appointment).ThenInclude(a => a!.Service)
                    .FirstOrDefaultAsync(p => p.Id == id);

                vm.DoctorName = full?.Doctor?.User?.Name;
                vm.PatientName = full?.Patient?.User?.Name;
                vm.AppointmentDate = full?.Appointment?.AppointmentDate;
                vm.AppointmentTime = full?.Appointment?.AppointmentTime;
                vm.ServiceName = full?.Appointment?.Service?.Name;

                await PopulateMedicinesDropdownAsync();
                if (vm.Items.Count == 0) vm.Items.Add(new MedicalReportPrescriptionItemViewModel());
                ViewData["ActivePage"] = "Doctors";
                return View(vm);
            }

            _context.PrescriptionItems.RemoveRange(prescription.Items);
            prescription.Notes = vm.Notes;

            foreach (var item in vm.Items.Where(i => i.MedicineId.HasValue))
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

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "تم تعديل الوصفة الطبية بنجاح";
            return RedirectToAction(nameof(Prescriptions), new { id = vm.DoctorId });
        }

        public async Task<IActionResult> DeletePrescription(int? id, string? from = null)
        {
            if (id == null) return NotFound();

            var prescription = await _context.Prescriptions
                .Include(p => p.Patient).ThenInclude(p => p.User)
                .Include(p => p.Appointment)
                .Include(p => p.Items)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (prescription == null) return NotFound();

            ViewData["ActivePage"] = "Doctors";
            ViewData["From"] = from;
            return View(prescription);
        }

        [HttpPost, ActionName("DeletePrescription")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeletePrescriptionConfirmed(int id, string? from = null)
        {
            var prescription = await _context.Prescriptions
                .Include(p => p.Items)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (prescription == null) return NotFound();

            var doctorId = prescription.DoctorId;

            var appointment = await _context.Appointments
                .FirstOrDefaultAsync(a => a.Id == prescription.AppointmentId);

            if (appointment != null)
                appointment.Status = "Booked";

            _context.PrescriptionItems.RemoveRange(prescription.Items);
            _context.Prescriptions.Remove(prescription);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "تم حذف الوصفة الطبية";

            if (from == "profile")
                return RedirectToAction(nameof(MyProfile));

            return RedirectToAction(nameof(Prescriptions), new { id = doctorId });
        }

        public async Task<IActionResult> MedicalReport(int? id, string? from = null)
        {
            if (id == null) return NotFound();

            var appointment = await GetAppointmentForReportAsync(id.Value);
            if (appointment == null) return NotFound();

            await PopulateMedicinesDropdownAsync();
            ViewData["ActivePage"] = "Doctors";
            ViewData["From"] = from;
            return View(MapToMedicalReportViewModel(appointment));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MedicalReport(int id, MedicalReportViewModel vm, string? from = null)
        {
            if (id != vm.AppointmentId) return NotFound();

            var appointment = await GetAppointmentForReportAsync(id);
            if (appointment == null) return NotFound();

            ValidatePrescriptionItems(vm.Items);

            if (!ModelState.IsValid)
            {
                await PopulateMedicinesDropdownAsync();
                ViewData["ActivePage"] = "Doctors";
                ViewData["From"] = from;
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

            if (from == "profile")
                return RedirectToAction(nameof(MyProfile));

            return RedirectToAction(nameof(MedicalReport), new { id = appointment.Id });
        }

        private void PopulateDropdowns()
        {
            ViewBag.Departments = new SelectList(
                _context.Departments.OrderBy(d => d.Name).ToList(),
                "Id",
                "Name");
        }

        private void ValidateCreatePassword(DoctorViewModel vm)
        {
            if (string.IsNullOrWhiteSpace(vm.Password))
            {
                ModelState.AddModelError(nameof(DoctorViewModel.Password), "كلمة المرور مطلوبة.");
            }

            if (string.IsNullOrWhiteSpace(vm.ConfirmPassword))
            {
                ModelState.AddModelError(nameof(DoctorViewModel.ConfirmPassword), "تأكيد كلمة المرور مطلوب.");
            }
        }

        private async Task AddDoctorRoleIfExistsAsync(User user)
        {
            var doctorRole = await _roleManager.Roles
                .Where(r => r.Name != null && r.Name.ToLower() == "doctor")
                .Select(r => r.Name)
                .FirstOrDefaultAsync();

            if (!string.IsNullOrWhiteSpace(doctorRole))
            {
                await _userManager.AddToRoleAsync(user, doctorRole);
            }
        }

        private async Task PopulateBookedAppointmentsDropdownAsync(int doctorId)
        {
            var appointments = await _context.Appointments
                .Include(a => a.Patient).ThenInclude(p => p.User)
                .Include(a => a.Service)
                .Where(a => a.DoctorId == doctorId && a.Status == "Booked")
                .OrderBy(a => a.AppointmentDate).ThenBy(a => a.AppointmentTime)
                .ToListAsync();

            var items = appointments.Select(a => new
            {
                a.Id,
                Text = $"{a.Patient.User.Name} — {a.AppointmentDate:yyyy/MM/dd} {a.AppointmentTime:hh\\:mm} — {a.Service.Name}"
            }).ToList();

            ViewBag.Appointments = new SelectList(items, "Id", "Text");
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
            if (vm.Items.Count == 0)
                vm.Items.Add(new MedicalReportPrescriptionItemViewModel());
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

            await using var stream = new FileStream(filePath, FileMode.Create);
            await file.CopyToAsync(stream);

            return $"/uploads/doctors/{fileName}";
        }

        private void AddIdentityErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }
    }
}
