using Medical.PL.Data.Models;
using Medical.PL.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Medical.PL.Controllers
{
    [Authorize]

    public class DoctorScheduleController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public DoctorScheduleController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // GET: DoctorSchedule
        public async Task<IActionResult> Index()
        {
            var schedules = await _unitOfWork.DoctorSchedules
                .GetAllWithIncludesAsync(
                    s => s.Doctor,
                    s => s.Doctor.User
                );

            return View(schedules);
        }

        // GET: DoctorSchedule/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var schedule = await _unitOfWork.DoctorSchedules
                .GetByIdWithIncludesAsync(id.Value,
                    s => s.Doctor,
                    s => s.Doctor.User
                );

            if (schedule == null) return NotFound();

            return View(schedule);
        }

        // GET: DoctorSchedule/Create
        public async Task<IActionResult> Create()
        {
            await PopulateDoctorsDropdown();
            ViewBag.Days = GetDaysList();
            return View();
        }

        // POST: DoctorSchedule/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(DoctorSchedule schedule)
        {
            if (ModelState.IsValid)
            {
                schedule.UpdatedAt = DateTime.UtcNow;
                await _unitOfWork.DoctorSchedules.AddAsync(schedule);
                await _unitOfWork.CompleteAsync();
                return RedirectToAction(nameof(Index));
            }

            await PopulateDoctorsDropdown(schedule.DoctorId);
            ViewBag.Days = GetDaysList();
            return View(schedule);
        }

        // GET: DoctorSchedule/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var schedule = await _unitOfWork.DoctorSchedules.GetByIdAsync(id.Value);
            if (schedule == null) return NotFound();

            await PopulateDoctorsDropdown(schedule.DoctorId);
            ViewBag.Days = GetDaysList();
            return View(schedule);
        }

        // POST: DoctorSchedule/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, DoctorSchedule schedule)
        {
            if (id != schedule.Id) return NotFound();

            if (ModelState.IsValid)
            {
                schedule.UpdatedAt = DateTime.UtcNow;
                _unitOfWork.DoctorSchedules.Update(schedule);
                await _unitOfWork.CompleteAsync();
                return RedirectToAction(nameof(Index));
            }

            await PopulateDoctorsDropdown(schedule.DoctorId);
            ViewBag.Days = GetDaysList();
            return View(schedule);
        }

        // GET: DoctorSchedule/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var schedule = await _unitOfWork.DoctorSchedules
                .GetByIdWithIncludesAsync(id.Value,
                    s => s.Doctor,
                    s => s.Doctor.User
                );

            if (schedule == null) return NotFound();

            return View(schedule);
        }

        // POST: DoctorSchedule/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var schedule = await _unitOfWork.DoctorSchedules.GetByIdAsync(id);
            if (schedule != null)
            {
                _unitOfWork.DoctorSchedules.Delete(schedule);
                await _unitOfWork.CompleteAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        // ========== Helpers ==========

        private async Task PopulateDoctorsDropdown(int? selectedId = null)
        {
            var doctors = await _unitOfWork.Doctors
                .GetAllWithIncludesAsync(d => d.User);

            var activeDoctors = doctors.Where(d => d.IsActive);

            ViewBag.Doctors = new SelectList(
                activeDoctors.Select(d => new
                {
                    d.Id,
                    Name = "د. " + d.User.Name + " - " + d.Specialization
                }),
                "Id", "Name", selectedId
            );
        }

        private List<SelectListItem> GetDaysList()
        {
            return new List<SelectListItem>
        {
            new SelectListItem { Value = "السبت", Text = "السبت" },
            new SelectListItem { Value = "الأحد", Text = "الأحد" },
            new SelectListItem { Value = "الاثنين", Text = "الاثنين" },
            new SelectListItem { Value = "الثلاثاء", Text = "الثلاثاء" },
            new SelectListItem { Value = "الأربعاء", Text = "الأربعاء" },
            new SelectListItem { Value = "الخميس", Text = "الخميس" },
            new SelectListItem { Value = "الجمعة", Text = "الجمعة" }
        };
        }
    }
}