using Medical.PL.Data.Models;
using Medical.PL.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NToastNotify;

namespace Medical.PL.Controllers
{
    [Authorize]
    public class MedicineController : Controller
    {
        private readonly IMedicineService _service;
        private readonly IToastNotification _toast;
        public MedicineController(IMedicineService service, IToastNotification toast)
        {
            _service = service;
            _toast = toast;
        }
        public async Task<IActionResult> Index(string searchTerm)
        {
            //var allMedicines = await _service.GetAllAsync();
            //return View(allMedicines);
            var medicines = await _service.SearchAsync(searchTerm);
            return View(medicines);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Create(Medicine medicine)
        {

            if (ModelState.IsValid)
            {
                await _service.AddAsync(medicine);
                //TempData["SuccessMessage"] = "Medicine created successfully!";
                _toast.AddSuccessToastMessage("تم إضافة الدواء بنجاح");
                return RedirectToAction("Index");
            }
            return View(medicine);
        }


        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id)
        {
            var medicine = await _service.GetByIdAsync(id);
            if (medicine == null)
            {
                return NotFound();
            }
            return View(medicine);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Edit(Medicine medicine)
        {
            if (ModelState.IsValid)
            {
                await _service.UpdateAsync(medicine);
                _toast.AddInfoToastMessage("تم تحديث بيانات الدواء بنجاح");
                return RedirectToAction("Index");
            }
            return View(medicine);
        }


        public async Task<IActionResult> Details(int id)
        {
            var medicine = await _service.GetByIdAsync(id);
            if (medicine == null)
            {
                return NotFound();
            }
            return View(medicine);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            await _service.DeleteAsync(id);
            _toast.AddErrorToastMessage("تم حذف الدواء بنجاح");
            return RedirectToAction("Index");
        }


    }
}
