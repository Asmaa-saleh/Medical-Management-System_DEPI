using Medical.PL.Data.Models;
using Medical.PL.Services;
using Microsoft.AspNetCore.Mvc;
using NToastNotify;

namespace Medical.PL.Controllers
{
    public class MedicineController : Controller
    {
        private readonly IMedicineService _service;
        private readonly IToastNotification _toast;
        public MedicineController(IMedicineService service, IToastNotification toast)
        {
            _service = service;
            _toast = toast;
        }
        public async Task<IActionResult> Index()
        {
            var allMedicines = await _service.GetAllAsync();
            return View(allMedicines);
        }
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }
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

        public async Task<IActionResult> Edit(int id)
        {
            var medicine = await _service.GetByIdAsync(id);
            if (medicine == null)
            {
                return NotFound();
            }
            return View(medicine);
        }
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

        
        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            await _service.DeleteAsync(id);
            _toast.AddErrorToastMessage("تم حذف الدواء بنجاح");
            return RedirectToAction("Index");
        }


    }
}
