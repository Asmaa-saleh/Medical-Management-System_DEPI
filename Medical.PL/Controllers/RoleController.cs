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


    public class RoleController : Controller
    {
        private readonly RoleManager<IdentityRole<int>> _roleManager;
        private readonly UserManager<User> _userManager;
        private readonly IToastNotification _toast;

        public RoleController(
            RoleManager<IdentityRole<int>> roleManager,
            UserManager<User> userManager,
            IToastNotification toast)
        {
            _roleManager = roleManager;
            _userManager = userManager;
            _toast = toast;
        }

        public async Task<IActionResult> Index()
        {
            var roles = await _roleManager.Roles
                .OrderBy(r => r.Name)
                .ToListAsync();

            var viewModel = new List<RoleViewModel>();
            foreach (var role in roles)
            {
                var users = string.IsNullOrWhiteSpace(role.Name)
                    ? []
                    : await _userManager.GetUsersInRoleAsync(role.Name);

                viewModel.Add(new RoleViewModel
                {
                    Id = role.Id,
                    Name = role.Name ?? string.Empty,
                    UsersCount = users.Count
                });
            }

            return View(viewModel);
        }

        public async Task<IActionResult> Details(int id)
        {
            var role = await _roleManager.FindByIdAsync(id.ToString());
            if (role == null)
            {
                return NotFound();
            }

            var users = string.IsNullOrWhiteSpace(role.Name)
                ? []
                : await _userManager.GetUsersInRoleAsync(role.Name);

            return View(new RoleViewModel
            {
                Id = role.Id,
                Name = role.Name ?? string.Empty,
                UsersCount = users.Count,
                Users = users.Select(u => u.Name).ToList()
            });
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View(new RoleViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(RoleViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var roleName = model.Name.Trim();
            var exists = await _roleManager.RoleExistsAsync(roleName);
            if (exists)
            {
                ModelState.AddModelError(nameof(model.Name), "هذا الدور موجود بالفعل");
                return View(model);
            }

            var result = await _roleManager.CreateAsync(new IdentityRole<int>(roleName));
            if (result.Succeeded)
            {
                _toast.AddSuccessToastMessage("تم إضافة الدور بنجاح");
                return RedirectToAction(nameof(Index));
            }

            AddIdentityErrors(result);
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var role = await _roleManager.FindByIdAsync(id.ToString());
            if (role == null)
            {
                return NotFound();
            }

            return View(new RoleViewModel
            {
                Id = role.Id,
                Name = role.Name ?? string.Empty
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, RoleViewModel model)
        {
            if (id != model.Id)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var role = await _roleManager.FindByIdAsync(id.ToString());
            if (role == null)
            {
                return NotFound();
            }

            role.Name = model.Name.Trim();
            var result = await _roleManager.UpdateAsync(role);
            if (result.Succeeded)
            {
                _toast.AddInfoToastMessage("تم تحديث الدور بنجاح");
                return RedirectToAction(nameof(Index));
            }

            AddIdentityErrors(result);
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var role = await _roleManager.FindByIdAsync(id.ToString());
            if (role == null)
            {
                return NotFound();
            }

            var users = string.IsNullOrWhiteSpace(role.Name)
                ? []
                : await _userManager.GetUsersInRoleAsync(role.Name);

            return View(new RoleViewModel
            {
                Id = role.Id,
                Name = role.Name ?? string.Empty,
                UsersCount = users.Count
            });
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var role = await _roleManager.FindByIdAsync(id.ToString());
            if (role == null)
            {
                return NotFound();
            }

            var users = string.IsNullOrWhiteSpace(role.Name)
                ? []
                : await _userManager.GetUsersInRoleAsync(role.Name);

            if (users.Count > 0)
            {
                ModelState.AddModelError(string.Empty, "لا يمكن حذف دور مرتبط بمستخدمين");
                return View(new RoleViewModel
                {
                    Id = role.Id,
                    Name = role.Name ?? string.Empty,
                    UsersCount = users.Count
                });
            }

            var result = await _roleManager.DeleteAsync(role);
            if (result.Succeeded)
            {
                _toast.AddErrorToastMessage("تم حذف الدور بنجاح");
                return RedirectToAction(nameof(Index));
            }

            AddIdentityErrors(result);
            return View(new RoleViewModel
            {
                Id = role.Id,
                Name = role.Name ?? string.Empty,
                UsersCount = users.Count
            });
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
