using Medical.PL.Data.Models;
using Medical.PL.Interfaces;
using Medical.PL.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NToastNotify;

namespace Medical.PL.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IToastNotification _toast;
        private readonly IUnitOfWork _unitOfWork;

        public AccountController(
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            IToastNotification toast,
            IUnitOfWork unitOfWork)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _toast = toast;
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            return RedirectToAction(nameof(SignIn));
        }

        [HttpGet]
        public IActionResult SignIn(string? returnUrl = null)
        {
            return View(new SignInViewModel { ReturnUrl = returnUrl });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SignIn(SignInViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "بيانات الدخول غير صحيحة");
                return View(model);
            }

            var result = await _signInManager.PasswordSignInAsync(
                user.UserName ?? user.Email ?? model.Email,
                model.Password,
                model.RememberMe,
                lockoutOnFailure: false);

            if (result.Succeeded)
            {
                _toast.AddSuccessToastMessage("تم تسجيل الدخول بنجاح");

                if (!string.IsNullOrWhiteSpace(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
                {
                    return LocalRedirect(model.ReturnUrl);
                }

                return RedirectToAction("LandingPage", "Home");
            }

            ModelState.AddModelError(string.Empty, "بيانات الدخول غير صحيحة");
            return View(model);
        }

        [HttpGet]
        public IActionResult SignUp()
        {
            return View(new SignUpViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SignUp(SignUpViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var existingUser = await _userManager.FindByEmailAsync(model.Email);

            if (existingUser != null)
            {
                ModelState.AddModelError("Email", "هذا البريد الإلكتروني مستخدم بالفعل");
                return View(model);
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

            var result = await _userManager.CreateAsync(user, model.Password);
            if (result.Succeeded)
            {
                var patient = new Patient
                {
                    UserId = user.Id
                };

                await _unitOfWork.Patients.AddAsync(patient);
                await _unitOfWork.CompleteAsync();

                await _signInManager.SignInAsync(user, isPersistent: false);
                _toast.AddSuccessToastMessage("تم إنشاء الحساب بنجاح");
                return RedirectToAction("LandingPage", "Home");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            _toast.AddInfoToastMessage("تم تسجيل الخروج");
            return RedirectToAction("LandingPage", "Home");
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Profile()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction(nameof(SignIn));
            }

            if (await _userManager.IsInRoleAsync(user, "Doctor"))
                return RedirectToAction("MyProfile", "Doctor");

            // map base user info
            var vm = new ProfileMedicalVM
            {
                UserId = user.Id,
                Name = user.Name,
                Email = user.Email ?? string.Empty,
                PhoneNumber = user.PhoneNumber ?? string.Empty,
                DateOfBirth = user.DateOfBirth,
                Gender = user.Gender,
                CreatedAt = user.CreatedAt
            };

            // try to get Patient record and related data
            var patients = await _unitOfWork.Patients.FindAsync(p => p.UserId == user.Id);
            var patient = patients.FirstOrDefault();
            if (patient != null)
            {
                var allAppointments = await _unitOfWork.Appointments.GetAllWithIncludesAsync(a => a.Doctor, a => a.Doctor.User, a => a.Service);
                var appointments = allAppointments.Where(a => a.PatientId == patient.Id);

                var allPrescriptions = await _unitOfWork.Prescriptions.GetAllWithIncludesAsync(pr => pr.Doctor, pr => pr.Doctor.User, pr => pr.Appointment, pr => pr.Items);
                var prescriptions = allPrescriptions.Where(pr => pr.PatientId == patient.Id);

                vm.Appointments = appointments.OrderByDescending(a => a.AppointmentDate).ToList();
                vm.Prescriptions = prescriptions.OrderByDescending(p => p.Id).ToList();
            }

            return View(vm);
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> EditProfile()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction(nameof(SignIn));
            }

            if (await _userManager.IsInRoleAsync(user, "Doctor"))
                return RedirectToAction("MyProfile", "Doctor");

            return View(MapToProfileViewModel(user));
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditProfile(UserViewModel model)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction(nameof(SignIn));
            }

            if (user.Id != model.Id)
            {
                return Forbid();
            }

            ModelState.Remove(nameof(model.Password));
            ModelState.Remove(nameof(model.ConfirmPassword));

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            user.Name = model.Name.Trim();
            user.UserName = model.Email.Trim();
            user.Email = model.Email.Trim();
            user.PhoneNumber = model.PhoneNumber.Trim();
            user.DateOfBirth = model.DateOfBirth;
            user.Gender = model.Gender;

            var result = await _userManager.UpdateAsync(user);
            if (result.Succeeded)
            {
                _toast.AddSuccessToastMessage("تم تحديث الملف الشخصي بنجاح");
                return RedirectToAction(nameof(Profile));
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return View(model);
        }

        public IActionResult AccessDenied()
        {
            return View();
        }

        private static UserViewModel MapToProfileViewModel(User user)
        {
            return new UserViewModel
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email ?? string.Empty,
                PhoneNumber = user.PhoneNumber ?? string.Empty,
                DateOfBirth = user.DateOfBirth,
                Gender = user.Gender,
                CreatedAt = user.CreatedAt
            };
        }
    }
}
