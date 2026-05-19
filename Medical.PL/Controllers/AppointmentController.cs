using Medical.PL.Data.Models;
using Medical.PL.Interfaces;
using Medical.PL.Repositories;
using Medical.PL.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using NToastNotify;
using X.PagedList.Extensions;

namespace Medical.PL.Controllers
{
    [Authorize]
    public class AppointmentController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IToastNotification _toast;
        public AppointmentController(IUnitOfWork unitOfWork, IToastNotification toast)
        {
            _unitOfWork = unitOfWork;
            _toast=toast;
        }

        public async Task<IActionResult> Index()
        {
            var appointments = await _unitOfWork.Appointments.GetAllWithIncludesAsync(
                a => a.Patient.User,
                a => a.Doctor.User,
                a => a.Service,
                a => a.DoctorSchedules
            );

            return View(appointments.OrderBy(a => a.AppointmentDate).ThenBy(a => a.AppointmentTime));
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var appointment = await GetAppointmentWithDetails(id.Value);
            if (appointment == null) return NotFound();

            return View(appointment);
        }

        public async Task<IActionResult> Create()
        {
            await PopulateFormLists();
            return View(new AppointmentFormViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AppointmentFormViewModel model)
        {
            var schedule = await ValidateAppointmentForm(model);
            if (!ModelState.IsValid || schedule == null)
            {
                await PopulateFormLists();
                return View(model);
            }

            var patientId = await ResolvePatientId(model);
            if (patientId == null)
            {
                await PopulateFormLists();
                return View(model);
            }

            var appointmentDate = model.AppointmentDate.Date;
            var appointment = new Appointment
            {
                PatientId = patientId.Value,
                DoctorId = schedule.DoctorId,
                ServiceId = model.ServiceId,
                ScheduleId = schedule.Id,
                AppointmentDate = appointmentDate,
                AppointmentTime = model.AppointmentTime,
                QueueNumber = await GetNextQueueNumber(schedule.Id, appointmentDate),
                BookingSource = model.BookingSource,
                Status = model.Status,
                Notes = model.Notes,
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.Appointments.AddAsync(appointment);
            await _unitOfWork.CompleteAsync();

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var appointment = await GetAppointmentWithDetails(id.Value);
            if (appointment == null) return NotFound();

            var model = new AppointmentFormViewModel
            {
                Id = appointment.Id,
                PatientMode = "Existing",
                ExistingPatientId = appointment.PatientId,
                ScheduleId = appointment.ScheduleId,
                ServiceId = appointment.ServiceId,
                AppointmentDate = appointment.AppointmentDate,
                AppointmentTime = appointment.AppointmentTime,
                BookingSource = appointment.BookingSource,
                Status = appointment.Status,
                Notes = appointment.Notes
            };

            await PopulateFormLists();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, AppointmentFormViewModel model)
        {
            if (id != model.Id) return NotFound();

            var appointment = await _unitOfWork.Appointments.GetByIdAsync(id);
            if (appointment == null) return NotFound();

            var schedule = await ValidateAppointmentForm(model, id);
            if (!ModelState.IsValid || schedule == null)
            {
                await PopulateFormLists();
                return View(model);
            }

            var patientId = await ResolvePatientId(model);
            if (patientId == null)
            {
                await PopulateFormLists();
                return View(model);
            }

            var appointmentDate = model.AppointmentDate.Date;
            var needsNewQueue = appointment.ScheduleId != schedule.Id || appointment.AppointmentDate.Date != appointmentDate;

            appointment.PatientId = patientId.Value;
            appointment.DoctorId = schedule.DoctorId;
            appointment.ServiceId = model.ServiceId;
            appointment.ScheduleId = schedule.Id;
            appointment.AppointmentDate = appointmentDate;
            appointment.AppointmentTime = model.AppointmentTime;
            appointment.BookingSource = model.BookingSource;
            appointment.Status = model.Status;
            appointment.Notes = model.Notes;

            if (needsNewQueue)
            {
                appointment.QueueNumber = await GetNextQueueNumber(schedule.Id, appointmentDate, appointment.Id);
            }

            _unitOfWork.Appointments.Update(appointment);
            await _unitOfWork.CompleteAsync();

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var appointment = await GetAppointmentWithDetails(id.Value);
            if (appointment == null) return NotFound();

            return View(appointment);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var appointment = await _unitOfWork.Appointments.GetByIdAsync(id);
            if (appointment != null)
            {
                _unitOfWork.Appointments.Delete(appointment);
                await _unitOfWork.CompleteAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        private async Task<Appointment?> GetAppointmentWithDetails(int id)
        {
            return await _unitOfWork.Appointments.GetByIdWithIncludesAsync(
                id,
                a => a.Patient.User,
                a => a.Doctor.User,
                a => a.Service,
                a => a.DoctorSchedules
            );
        }

        private async Task PopulateFormLists()
        {
            var patients = await _unitOfWork.Patients.GetAllWithIncludesAsync(p => p.User);
            var schedules = await _unitOfWork.DoctorSchedules.GetAllWithIncludesAsync(s => s.Doctor.User);
            var services = await _unitOfWork.Services.GetAllAsync();

            ViewBag.Patients = patients
                .OrderBy(p => p.User.Name)
                .ToList();

            ViewBag.Schedules = new SelectList(
                schedules
                    .Where(s => s.Doctor.IsActive)
                    .OrderBy(s => s.Doctor.User.Name)
                    .ThenBy(s => s.DayOfWeek)
                    .Select(s => new
                    {
                        s.Id,
                        Name = $"د. {s.Doctor.User.Name} - {s.DayOfWeek} ({s.StartTime:hh\\:mm} - {s.EndTime:hh\\:mm})"
                    }),
                "Id",
                "Name"
            );

            ViewBag.Services = new SelectList(
                services.Where(s => !s.IsDeleted).OrderBy(s => s.Name),
                "Id",
                "Name"
            );

            ViewBag.BookingSources = new SelectList(new[]
            {
                new { Value = "Clinic", Text = "داخل العيادة" },
                new { Value = "Online", Text = "أونلاين" }
            }, "Value", "Text");

            ViewBag.Statuses = new SelectList(new[]
            {
                new { Value = "Booked", Text = "محجوز" },
                new { Value = "Completed", Text = "تم الكشف" },
                new { Value = "Cancelled", Text = "ملغي" }
            }, "Value", "Text");
        }

        private async Task<DoctorSchedule?> ValidateAppointmentForm(AppointmentFormViewModel model, int? currentAppointmentId = null)
        {
            if (model.PatientMode == "Existing" && model.ExistingPatientId == null)
            {
                ModelState.AddModelError(nameof(model.ExistingPatientId), "اختر المريض السابق.");
            }

            if (model.PatientMode == "New")
            {
                if (string.IsNullOrWhiteSpace(model.NewPatientName))
                    ModelState.AddModelError(nameof(model.NewPatientName), "اسم المريض مطلوب.");
                if (model.NewPatientDateOfBirth == null)
                    ModelState.AddModelError(nameof(model.NewPatientDateOfBirth), "تاريخ الميلاد مطلوب.");
                if (string.IsNullOrWhiteSpace(model.NewPatientPhone))
                    ModelState.AddModelError(nameof(model.NewPatientPhone), "رقم التليفون مطلوب.");
                if (string.IsNullOrWhiteSpace(model.NewPatientEmail))
                    ModelState.AddModelError(nameof(model.NewPatientEmail), "البريد الإلكتروني مطلوب.");
                if (string.IsNullOrWhiteSpace(model.NewPatientGender))
                    ModelState.AddModelError(nameof(model.NewPatientGender), "النوع مطلوب.");
            }

            if (model.ScheduleId <= 0)
            {
                ModelState.AddModelError(nameof(model.ScheduleId), "اختر موعد الطبيب.");
                return null;
            }

            if (model.ServiceId <= 0)
            {
                ModelState.AddModelError(nameof(model.ServiceId), "اختر الخدمة.");
            }

            var schedule = await _unitOfWork.DoctorSchedules.GetByIdWithIncludesAsync(
                model.ScheduleId,
                s => s.Doctor.User
            );

            if (schedule == null)
            {
                ModelState.AddModelError(nameof(model.ScheduleId), "موعد الطبيب غير موجود.");
                return null;
            }

            if (model.AppointmentDate == default)
            {
                ModelState.AddModelError(nameof(model.AppointmentDate), "تاريخ الحجز مطلوب.");
            }
            else
            {
                var expectedDay = GetArabicDayName(model.AppointmentDate.DayOfWeek);
                if (schedule.DayOfWeek != expectedDay)
                {
                    ModelState.AddModelError(nameof(model.AppointmentDate), $"التاريخ المختار يجب أن يكون يوم {schedule.DayOfWeek}.");
                }
            }

            if (model.AppointmentTime < schedule.StartTime || model.AppointmentTime > schedule.EndTime)
            {
                ModelState.AddModelError(nameof(model.AppointmentTime), "وقت الحجز يجب أن يكون داخل وقت عمل الطبيب.");
            }

            var appointments = await _unitOfWork.Appointments.FindAsync(a =>
                a.ScheduleId == schedule.Id &&
                a.AppointmentDate == model.AppointmentDate.Date
            );

            var appointmentsOnSlot = appointments
                .Where(a => currentAppointmentId == null || a.Id != currentAppointmentId.Value)
                .ToList();

            if (appointmentsOnSlot.Count >= schedule.MaxPatients)
            {
                ModelState.AddModelError(string.Empty, "تم الوصول للحد الأقصى لعدد المرضى في هذا الموعد.");
            }

            if (model.BookingSource == "Online" && appointmentsOnSlot.Count(a => a.BookingSource == "Online") >= schedule.MaxOnlineBooking)
            {
                ModelState.AddModelError(nameof(model.BookingSource), "تم الوصول للحد الأقصى للحجوزات الأونلاين في هذا الموعد.");
            }

            return schedule;
        }

        private async Task<int?> ResolvePatientId(AppointmentFormViewModel model)
        {
            if (model.PatientMode == "Existing")
            {
                return model.ExistingPatientId;
            }

            var user = new User
            {
                Name = model.NewPatientName!.Trim(),
                DateOfBirth = model.NewPatientDateOfBirth!.Value,
                Email = model.NewPatientEmail!.Trim(),
                PhoneNumber = model.NewPatientPhone!.Trim(),
                Gender = model.NewPatientGender!,
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.Users.AddAsync(user);
            await _unitOfWork.CompleteAsync();

            var patient = new Patient
            {
                UserId = user.Id
            };

            await _unitOfWork.Patients.AddAsync(patient);
            await _unitOfWork.CompleteAsync();

            return patient.Id;
        }

        private async Task<int> GetNextQueueNumber(int scheduleId, DateTime appointmentDate, int? currentAppointmentId = null)
        {
            var appointments = await _unitOfWork.Appointments.FindAsync(a =>
                a.ScheduleId == scheduleId &&
                a.AppointmentDate == appointmentDate.Date
            );

            return appointments
                .Where(a => currentAppointmentId == null || a.Id != currentAppointmentId.Value)
                .Select(a => a.QueueNumber)
                .DefaultIfEmpty(0)
                .Max() + 1;
        }

        private static string GetArabicDayName(DayOfWeek day)
        {
            return day switch
            {
                DayOfWeek.Saturday => "السبت",
                DayOfWeek.Sunday => "الأحد",
                DayOfWeek.Monday => "الاثنين",
                DayOfWeek.Tuesday => "الثلاثاء",
                DayOfWeek.Wednesday => "الأربعاء",
                DayOfWeek.Thursday => "الخميس",
                DayOfWeek.Friday => "الجمعة",
                _ => string.Empty
            };
        }
        

        public async Task<IActionResult> Booking(int? departmentId, int page = 1)
        {
            int pageSize = 8;

            // Get services with department included
            var services = await _unitOfWork.Services
                .GetAllWithIncludesAsync(s => s.Department);

            // Base query (remove deleted)
            var query = services
                .Where(s => !s.IsDeleted)
                .AsQueryable();

            // Filter by department if selected
            if (departmentId.HasValue)
            {
                query = query.Where(s => s.DepartmentId == departmentId.Value);
            }

            // Pagination
            var model = query
                .OrderBy(s => s.Id)
                .ToPagedList(page, pageSize);

            // Get only departments that have services
            var validDepartmentIds = services
                .Where(s => !s.IsDeleted)
                .Select(s => s.DepartmentId)
                .Distinct()
                .ToList();

            var departments = await _unitOfWork.Departments.GetAllAsync();

            ViewBag.Departments = departments
                .Where(d => !d.IsDeleted && validDepartmentIds.Contains(d.Id))
                .ToList();

            ViewBag.SelectedDepartment = departmentId;

            return View(model);
        }

        public async Task<IActionResult> SelectDoctor(int serviceId)
        {
            // Get selected service
            var service = await _unitOfWork.Services
                .GetByIdWithIncludesAsync(serviceId,s => s.Department);

            if (service == null)
                return NotFound();

            // Get doctors in same department
            var doctors = await _unitOfWork.Doctors
                .GetAllWithIncludesAsync(
                    d => d.User,
                    d => d.Department
                );

            var model = doctors
                .Where(d =>
                    d.IsActive &&
                    d.DepartmentId == service.DepartmentId)
                .ToList();

            ViewBag.Service = service;

            return View(model);
        }

       
        public async Task<IActionResult> SelectTime(int doctorId, int serviceId, DateTime? selectedDate)
        {
            var doctor = await _unitOfWork.Doctors
                .GetByIdWithIncludesAsync(
                    doctorId,
                    d => d.User
                );

            if (doctor == null)
                return NotFound();

            var date = selectedDate ?? DateTime.Today;

            var arabicDay = GetArabicDayName(date.DayOfWeek);

            var schedules = await _unitOfWork.DoctorSchedules
                .GetAllAsync();

            var schedule = schedules.FirstOrDefault(s =>
                s.DoctorId == doctorId &&
                s.DayOfWeek == arabicDay);

            var model = new SelectTimeVM
            {
                DoctorId = doctorId,
                ServiceId = serviceId,
                SelectedDate = date
            };
            model.AvailableDays = schedules
                .Where(s => s.DoctorId == doctorId)
                .Select(s => s.DayOfWeek)
                .Distinct()
                .ToList();

            if (schedule != null)
            {
                var appointments = await _unitOfWork.Appointments
                    .FindAsync(a =>
                        a.DoctorId == doctorId &&
                        a.AppointmentDate == date.Date
                    );

                var bookedTimes = appointments
                    .Select(a => a.AppointmentTime)
                    .ToList();

                // مدة الكشف
                int slotDuration = 30;

                for (
                    var time = schedule.StartTime;
                    time < schedule.EndTime;
                    time = time.Add(TimeSpan.FromMinutes(slotDuration))
                )
                {
                    model.Slots.Add(new TimeSlotVM
                    {
                        ScheduleId = schedule.Id,
                        Time = time,
                        IsBooked = bookedTimes.Contains(time)
                    });
                }
            }

            ViewBag.Doctor = doctor;

            return View(model);
        }

        public async Task<IActionResult> ConfirmBooking(int serviceId,int doctorId,int scheduleId,DateTime date,TimeSpan time)
        {
            var userId = int.Parse(
                User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value
            );

            var patient = await _unitOfWork.Patients
                .GetAllWithIncludesAsync(p => p.User);

            var currentPatient = patient
                .FirstOrDefault(p => p.UserId == userId);

            if (currentPatient == null)
                //return RedirectToAction("Login", "Account");
                return RedirectToAction("SignIn", "Account");

            var service = await _unitOfWork.Services
                .GetByIdAsync(serviceId);

            var doctor = await _unitOfWork.Doctors
                .GetByIdWithIncludesAsync(
                    doctorId,
                    d => d.User
                );

            var schedule = await _unitOfWork.DoctorSchedules
                .GetByIdAsync(scheduleId);

            if (service == null || doctor == null || schedule == null)
                return NotFound();

            var model = new ConfirmBookingVM
            {
                ServiceId = serviceId,
                DoctorId = doctorId,
                ScheduleId = scheduleId,

                Date = date,
                Time = time,

                ServiceName = service.Name,
                DoctorName = doctor.User.Name,
                DayName = schedule.DayOfWeek,

                Patient = new PatientVM
                {
                    Id = currentPatient.Id,
                    UserId = currentPatient.UserId,
                    Name = currentPatient.User.Name,
                    Email = currentPatient.User.Email,
                    Phone = currentPatient.User.PhoneNumber,
                    Gender = currentPatient.User.Gender,
                    DateOfBirth = currentPatient.User.DateOfBirth
                }
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ConfirmBooking(ConfirmBookingVM model)
        {
            var userId = int.Parse(
                User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value
            );

            var patients = await _unitOfWork.Patients
            .GetAllWithIncludesAsync(p => p.User);

            var patient = patients
                .FirstOrDefault(p => p.UserId == userId);

            if (patient == null)
            {
                return RedirectToAction("SignIn", "Account");
            }

            var appointment = new Appointment
            {
                PatientId = patient.Id,
                DoctorId = model.DoctorId,
                ServiceId = model.ServiceId,
                ScheduleId = model.ScheduleId,
                AppointmentDate = model.Date.Date,
                AppointmentTime = model.Time,

                QueueNumber = await GetNextQueueNumber(
                    model.ScheduleId,
                    model.Date
                ),

                Status = "Booked",
                BookingSource = "Online",
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.Appointments.AddAsync(appointment);
            await _unitOfWork.CompleteAsync();
            _toast.AddSuccessToastMessage("تم الحجز بنجاح");
            return RedirectToAction("LandingPage", "Home");
        }
        
    }
}
