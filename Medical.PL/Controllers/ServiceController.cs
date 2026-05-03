using Medical.PL.Data.Models;
using Medical.PL.Interfaces;
using Medical.PL.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Medical.PL.Controllers
{
    public class ServiceController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public ServiceController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IActionResult> Index()
        {
            var services = await _unitOfWork.Services.GetAllWithIncludesAsync(s => s.Department);

            return View(services
                .Where(s => !s.IsDeleted)
                .OrderBy(s => s.Department.Name)
                .ThenBy(s => s.Name));
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var service = await GetServiceWithDepartment(id.Value);
            if (service == null || service.IsDeleted) return NotFound();

            return View(service);
        }

        public async Task<IActionResult> Create()
        {
            await PopulateDepartmentsDropdown();
            return View(new ServiceFormViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ServiceFormViewModel model)
        {
            await ValidateDepartment(model.DepartmentId);

            if (!ModelState.IsValid)
            {
                await PopulateDepartmentsDropdown(model.DepartmentId);
                return View(model);
            }

            var service = new Service
            {
                Name = model.Name.Trim(),
                Description = string.IsNullOrWhiteSpace(model.Description) ? null : model.Description.Trim(),
                Price = model.Price,
                DepartmentId = model.DepartmentId
            };

            await _unitOfWork.Services.AddAsync(service);
            await _unitOfWork.CompleteAsync();

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var service = await _unitOfWork.Services.GetByIdAsync(id.Value);
            if (service == null || service.IsDeleted) return NotFound();

            var model = new ServiceFormViewModel
            {
                Id = service.Id,
                Name = service.Name,
                Description = service.Description,
                Price = service.Price,
                DepartmentId = service.DepartmentId
            };

            await PopulateDepartmentsDropdown(model.DepartmentId);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ServiceFormViewModel model)
        {
            if (id != model.Id) return NotFound();

            await ValidateDepartment(model.DepartmentId);

            if (!ModelState.IsValid)
            {
                await PopulateDepartmentsDropdown(model.DepartmentId);
                return View(model);
            }

            var service = await _unitOfWork.Services.GetByIdAsync(id);
            if (service == null || service.IsDeleted) return NotFound();

            service.Name = model.Name.Trim();
            service.Description = string.IsNullOrWhiteSpace(model.Description) ? null : model.Description.Trim();
            service.Price = model.Price;
            service.DepartmentId = model.DepartmentId;

            _unitOfWork.Services.Update(service);
            await _unitOfWork.CompleteAsync();

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var service = await GetServiceWithDepartment(id.Value);
            if (service == null || service.IsDeleted) return NotFound();

            return View(service);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var service = await _unitOfWork.Services.GetByIdAsync(id);
            if (service != null && !service.IsDeleted)
            {
                service.IsDeleted = true;
                _unitOfWork.Services.Update(service);
                await _unitOfWork.CompleteAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        private async Task<Service?> GetServiceWithDepartment(int id)
        {
            return await _unitOfWork.Services.GetByIdWithIncludesAsync(id, s => s.Department);
        }

        private async Task PopulateDepartmentsDropdown(int? selectedId = null)
        {
            var departments = await _unitOfWork.Departments.GetAllAsync();

            ViewBag.Departments = new SelectList(
                departments
                    .Where(d => !d.IsDeleted)
                    .OrderBy(d => d.Name),
                "Id",
                "Name",
                selectedId
            );
        }

        private async Task ValidateDepartment(int departmentId)
        {
            var department = await _unitOfWork.Departments.GetByIdAsync(departmentId);
            if (department == null || department.IsDeleted)
            {
                ModelState.AddModelError(nameof(ServiceFormViewModel.DepartmentId), "اختر قسم صحيح.");
            }
        }
    }
}
