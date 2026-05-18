using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace Medical.PL.ViewModels
{
    public class DoctorViewModel
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        [Required(ErrorMessage = "الاسم الكامل مطلوب.")]
        [StringLength(100, ErrorMessage = "الاسم لا يزيد عن 100 حرف.")]
        [Display(Name = "الاسم الكامل")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "البريد الإلكتروني مطلوب.")]
        [EmailAddress(ErrorMessage = "البريد الإلكتروني غير صحيح.")]
        [StringLength(150, ErrorMessage = "البريد الإلكتروني لا يزيد عن 150 حرف.")]
        [Display(Name = "البريد الإلكتروني")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "رقم الهاتف مطلوب.")]
        [Phone(ErrorMessage = "رقم الهاتف غير صحيح.")]
        [StringLength(20, ErrorMessage = "رقم الهاتف لا يزيد عن 20 رقمًا.")]
        [Display(Name = "رقم الهاتف")]
        public string PhoneNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = "تاريخ الميلاد مطلوب.")]
        [DataType(DataType.Date)]
        [Display(Name = "تاريخ الميلاد")]
        public DateTime DateOfBirth { get; set; }

        [Required(ErrorMessage = "النوع مطلوب.")]
        [StringLength(10, ErrorMessage = "النوع لا يزيد عن 10 أحرف.")]
        [Display(Name = "النوع")]
        public string Gender { get; set; } = string.Empty;

        [DataType(DataType.Password)]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "كلمة المرور يجب ألا تقل عن 6 أحرف.")]
        [Display(Name = "كلمة المرور")]
        public string? Password { get; set; }

        [DataType(DataType.Password)]
        [Compare(nameof(Password), ErrorMessage = "كلمة المرور وتأكيدها غير متطابقين.")]
        [Display(Name = "تأكيد كلمة المرور")]
        public string? ConfirmPassword { get; set; }

        [Required(ErrorMessage = "القسم مطلوب.")]
        [Display(Name = "القسم")]
        public int DepartmentId { get; set; }

        [Required(ErrorMessage = "التخصص مطلوب.")]
        [MaxLength(100, ErrorMessage = "التخصص لا يزيد عن 100 حرف.")]
        [Display(Name = "التخصص")]
        public string Specialization { get; set; } = string.Empty;

        [Required(ErrorMessage = "عدد سنوات الخبرة مطلوب.")]
        [Range(0, 60, ErrorMessage = "عدد سنوات الخبرة يجب أن يكون من 0 إلى 60.")]
        [Display(Name = "عدد سنوات الخبرة")]
        public int ExperienceYears { get; set; }

        [MaxLength(500, ErrorMessage = "النبذة لا تزيد عن 500 حرف.")]
        [Display(Name = "نبذة مختصرة")]
        public string? Bio { get; set; }

        [Display(Name = "الصورة الشخصية")]
        public IFormFile? ImageFile { get; set; }

        public string? Image { get; set; }

        [Display(Name = "نشط")]
        public bool IsActive { get; set; } = true;
    }
}
