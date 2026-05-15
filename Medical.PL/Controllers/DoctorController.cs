using Medical.PL.Data.Context;
using Medical.PL.Data.Models;
using Medical.PL.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using NToastNotify;



namespace Medical.PL.Controllers
{
    public class DoctorsController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;

        public DoctorsController(AppDbContext context, IWebHostEnvironment env)
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
            return View(doctor);
        }

        // GET: Doctors/Create
        public IActionResult Create()
        {
            PopulateDropdowns();
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

        // Helper Methods
        private void PopulateDropdowns()
        {
            ViewBag.Users = new SelectList(_context.Users, "Id", "UserName");
            ViewBag.Departments = new SelectList(_context.Departments, "Id", "Name");
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