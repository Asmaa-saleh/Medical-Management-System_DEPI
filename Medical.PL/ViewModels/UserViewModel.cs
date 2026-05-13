using System.ComponentModel.DataAnnotations;

namespace Medical.PL.ViewModels
{
    public class UserViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "الاسم مطلوب")]
        [StringLength(100, ErrorMessage = "الاسم لا يجب أن يتجاوز 100 حرف")]
        [Display(Name = "الاسم الكامل")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "البريد الإلكتروني مطلوب")]
        [EmailAddress(ErrorMessage = "برجاء إدخال بريد إلكتروني صحيح")]
        [StringLength(150, ErrorMessage = "البريد الإلكتروني لا يجب أن يتجاوز 150 حرف")]
        [Display(Name = "البريد الإلكتروني")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "رقم الهاتف مطلوب")]
        [Phone(ErrorMessage = "برجاء إدخال رقم هاتف صحيح")]
        [StringLength(20, ErrorMessage = "رقم الهاتف لا يجب أن يتجاوز 20 رقم")]
        [Display(Name = "رقم الهاتف")]
        public string PhoneNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = "تاريخ الميلاد مطلوب")]
        [DataType(DataType.Date)]
        [Display(Name = "تاريخ الميلاد")]
        public DateTime DateOfBirth { get; set; }

        [Required(ErrorMessage = "النوع مطلوب")]
        [StringLength(10, ErrorMessage = "النوع لا يجب أن يتجاوز 10 أحرف")]
        [Display(Name = "النوع")]
        public string Gender { get; set; } = string.Empty;

        [DataType(DataType.Password)]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "كلمة المرور يجب ألا تقل عن 6 أحرف")]
        [Display(Name = "كلمة المرور")]
        public string? Password { get; set; }

        [DataType(DataType.Password)]
        [Compare(nameof(Password), ErrorMessage = "كلمة المرور وتأكيدها غير متطابقين")]
        [Display(Name = "تأكيد كلمة المرور")]
        public string? ConfirmPassword { get; set; }

        public DateTime CreatedAt { get; set; }
        public bool IsPatient { get; set; }
        public bool IsDoctor { get; set; }
        public List<string> Roles { get; set; } = [];
        public List<string> SelectedRoles { get; set; } = [];
        public List<string> AvailableRoles { get; set; } = [];
    }
}
