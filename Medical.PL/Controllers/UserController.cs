using Medical.PL.Data.Context;
using Medical.PL.Data.Models;
using Medical.PL.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NToastNotify;

namespace Medical.PL.Controllers
{
    [Authorize(Roles = "Admin")]
    public class UserController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole<int>> _roleManager;
        private readonly AppDbContext _context;
        private readonly IToastNotification _toast;

        public UserController(
            UserManager<User> userManager,
            RoleManager<IdentityRole<int>> roleManager,
            AppDbContext context,
            IToastNotification toast)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _context = context;
            _toast = toast;
        }

        public async Task<IActionResult> Index()
        {
            var users = await _userManager.Users
                .Include(u => u.Patient)
                .Include(u => u.Doctor)
                .OrderBy(u => u.Name)
                .ToListAsync();

            var viewModel = new List<UserViewModel>();
            foreach (var user in users)
            {
                viewModel.Add(await MapToViewModel(user));
            }

            return View(viewModel);
        }

        public async Task<IActionResult> Details(int id)
        {
            var user = await GetUserWithRelations(id);
            if (user == null)
            {
                return NotFound();
            }

            return View(await MapToViewModel(user));
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            return View(await PrepareFormModel(new UserViewModel
            {
                DateOfBirth = DateTime.Today.AddYears(-18)
            }));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(UserViewModel model)
        {
            if (string.IsNullOrWhiteSpace(model.Password))
            {
                ModelState.AddModelError(nameof(model.Password), "كلمة المرور مطلوبة");
            }

            if (string.IsNullOrWhiteSpace(model.ConfirmPassword))
            {
                ModelState.AddModelError(nameof(model.ConfirmPassword), "تأكيد كلمة المرور مطلوب");
            }

            if (!ModelState.IsValid)
            {
                return View(await PrepareFormModel(model));
            }

            var user = new User
            {
                Name = model.Name.Trim(),
                UserName = model.Email.Trim(),
                Email = model.Email.Trim(),
                PhoneNumber = model.PhoneNumber.Trim(),
                DateOfBirth = model.DateOfBirth,
                Gender = model.Gender,
                CreatedAt = DateTime.UtcNow
            };

            var result = await _userManager.CreateAsync(user, model.Password!);
            if (!result.Succeeded)
            {
                AddIdentityErrors(result);
                return View(await PrepareFormModel(model));
            }

            await UpdateUserRoles(user, model.SelectedRoles);
            _toast.AddSuccessToastMessage("تم إضافة المستخدم بنجاح");
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var user = await GetUserWithRelations(id);
            if (user == null)
            {
                return NotFound();
            }

            var model = await MapToViewModel(user);
            model.SelectedRoles = model.Roles.ToList();
            return View(await PrepareFormModel(model));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, UserViewModel model)
        {
            if (id != model.Id)
            {
                return BadRequest();
            }

            ModelState.Remove(nameof(model.Password));
            ModelState.Remove(nameof(model.ConfirmPassword));

            if (!ModelState.IsValid)
            {
                return View(await PrepareFormModel(model));
            }

            var user = await _userManager.FindByIdAsync(id.ToString());
            if (user == null)
            {
                return NotFound();
            }

            user.Name = model.Name.Trim();
            user.UserName = model.Email.Trim();
            user.Email = model.Email.Trim();
            user.PhoneNumber = model.PhoneNumber.Trim();
            user.DateOfBirth = model.DateOfBirth;
            user.Gender = model.Gender;

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                AddIdentityErrors(result);
                return View(await PrepareFormModel(model));
            }

            await UpdateUserRoles(user, model.SelectedRoles);
            _toast.AddInfoToastMessage("تم تحديث المستخدم بنجاح");
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var user = await GetUserWithRelations(id);
            if (user == null)
            {
                return NotFound();
            }

            return View(await MapToViewModel(user));
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var user = await GetUserWithRelations(id);
            if (user == null)
            {
                return NotFound();
            }

            if (user.Patient != null || user.Doctor != null)
            {
                ModelState.AddModelError(string.Empty, "لا يمكن حذف مستخدم مرتبط ببيانات مريض أو طبيب");
                return View(await MapToViewModel(user));
            }

            var result = await _userManager.DeleteAsync(user);
            if (result.Succeeded)
            {
                _toast.AddErrorToastMessage("تم حذف المستخدم بنجاح");
                return RedirectToAction(nameof(Index));
            }

            AddIdentityErrors(result);
            return View(await MapToViewModel(user));
        }

        private async Task<User?> GetUserWithRelations(int id)
        {
            return await _context.Users
                .Include(u => u.Patient)
                .Include(u => u.Doctor)
                .FirstOrDefaultAsync(u => u.Id == id);
        }

        private async Task<UserViewModel> MapToViewModel(User user)
        {
            var roles = await _userManager.GetRolesAsync(user);

            return new UserViewModel
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email ?? string.Empty,
                PhoneNumber = user.PhoneNumber ?? string.Empty,
                DateOfBirth = user.DateOfBirth,
                Gender = user.Gender,
                CreatedAt = user.CreatedAt,
                Roles = roles.ToList(),
                IsPatient = user.Patient != null,
                IsDoctor = user.Doctor != null
            };
        }

        private async Task<UserViewModel> PrepareFormModel(UserViewModel model)
        {
            model.AvailableRoles = await _roleManager.Roles
                .OrderBy(r => r.Name)
                .Select(r => r.Name!)
                .ToListAsync();

            return model;
        }

        private async Task UpdateUserRoles(User user, List<string> selectedRoles)
        {
            var currentRoles = await _userManager.GetRolesAsync(user);
            var rolesToRemove = currentRoles.Except(selectedRoles).ToList();
            var rolesToAdd = selectedRoles.Except(currentRoles).ToList();

            if (rolesToRemove.Count > 0)
            {
                await _userManager.RemoveFromRolesAsync(user, rolesToRemove);
            }

            if (rolesToAdd.Count > 0)
            {
                await _userManager.AddToRolesAsync(user, rolesToAdd);
            }
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
