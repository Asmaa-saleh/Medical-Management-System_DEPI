
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace Medical.PL.ViewModels
{
    public class DoctorViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "مطلوب المستخدم")]
        [Display(Name = "المستخدم")]
        public string UserId { get; set; }

        [Required(ErrorMessage = "مطلوب القسم")]
        [Display(Name = "القسم")]
        public int DepartmentId { get; set; }

        [Required(ErrorMessage = "مطلوب التخصص")]
        [MaxLength(100)]
        [Display(Name = "التخصص")]
        public string Specialization { get; set; }

        [Required(ErrorMessage = "مطلوب عدد سنوات الخبرة")]
        [Range(0, 60, ErrorMessage = "يجب ان تكون عدد سنوات الخبرة من 0 ل60 سنة")]
        [Display(Name = "عدد سنوات الخبرة")]
        public int ExperienceYears { get; set; }

        [MaxLength(1000)]
        [Display(Name = "النبذة")]
        public string? Bio { get; set; }

        [Display(Name = "صورة البروفايل")]
        public IFormFile? ImageFile { get; set; }   // للرفع

        public string? Image { get; set; }           // للعرض

        [Display(Name = "نشط")]
        public bool IsActive { get; set; } = true;
    }
}