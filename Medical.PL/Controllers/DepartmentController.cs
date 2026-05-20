using Medical.PL.Data.Models;
using Medical.PL.Interfaces;
using Medical.PL.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Medical.PL.Controllers
{
    [Authorize]

    public class DepartmentController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public DepartmentController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IActionResult> Index()
        {
            //var departments = await _unitOfWork.Departments.FindAsync(d => !d.IsDeleted);
            var departments = await _unitOfWork.Departments.GetAllAsync();
            var viewModel = departments.Select(d => new DepartmentViewModel
            {
                Id = d.Id,
                Name = d.Name,
                Description = d.Description,
                IsDeleted = d.IsDeleted
            }).ToList();

            return View(viewModel);
        }

        public async Task<IActionResult> Details(int id)
        {
            var department = await _unitOfWork.Departments.GetByIdAsync(id);
            if (department == null || department.IsDeleted) return NotFound();

            var viewModel = new DepartmentViewModel
            {
                Id = department.Id,
                Name = department.Name,
                Description = department.Description
            };
            return View(viewModel);
        }


        [Authorize(Roles = "Admin")]
        public IActionResult Create() => View();


        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(DepartmentViewModel departmentVM)
        {
            if (!ModelState.IsValid) return View(departmentVM);

            var department = new Department
            {
                Name = departmentVM.Name,
                Description = departmentVM.Description
            };

            await _unitOfWork.Departments.AddAsync(department);
            await _unitOfWork.CompleteAsync();
            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id)
        {
            var department = await _unitOfWork.Departments.GetByIdAsync(id);
            if (department == null || department.IsDeleted) return NotFound();

            var viewModel = new DepartmentViewModel
            {
                Id = department.Id,
                Name = department.Name,
                Description = department.Description
            };
            return View(viewModel);
        }


        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, DepartmentViewModel departmentVM)
        {
            if (id != departmentVM.Id) return BadRequest();
            if (!ModelState.IsValid) return View(departmentVM);

            var departmentInDb = await _unitOfWork.Departments.GetByIdAsync(id);
            if (departmentInDb == null) return NotFound();

            departmentInDb.Name = departmentVM.Name;
            departmentInDb.Description = departmentVM.Description;

            _unitOfWork.Departments.Update(departmentInDb);
            await _unitOfWork.CompleteAsync();
            return RedirectToAction(nameof(Index));
        }


        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var department = await _unitOfWork.Departments.GetByIdAsync(id);
            if (department == null || department.IsDeleted) return NotFound();

            var viewModel = new DepartmentViewModel
            {
                Id = department.Id,
                Name = department.Name,
                Description = department.Description
            };
            return View(viewModel);
        }


        [Authorize(Roles = "Admin")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var department = await _unitOfWork.Departments.GetByIdAsync(id);
            if (department == null) return NotFound();

            department.IsDeleted = true;
            _unitOfWork.Departments.Update(department);
            await _unitOfWork.CompleteAsync();
            return RedirectToAction(nameof(Index));
        }


        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Restore(int id)
        {
            var department = await _unitOfWork.Departments.GetByIdAsync(id);

            if (department == null)
                return NotFound();

            department.IsDeleted = false;

            _unitOfWork.Departments.Update(department);

            await _unitOfWork.CompleteAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}