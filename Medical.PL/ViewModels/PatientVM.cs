using System.ComponentModel.DataAnnotations;

namespace Medical.PL.ViewModels
{
    public class PatientVM
    {
        public int Id { get; set; }
        public int UserId { get; set; }

        [Required(ErrorMessage = "الاسم مطلوب.")]
        [StringLength(100, ErrorMessage = "الاسم لا يزيد عن 100 حرف.")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "تاريخ الميلاد مطلوب.")]
        public DateTime DateOfBirth { get; set; }

        [Required(ErrorMessage = "البريد الإلكتروني مطلوب.")]
        [EmailAddress(ErrorMessage = "البريد الإلكتروني غير صحيح.")]
        [StringLength(150, ErrorMessage = "البريد الإلكتروني لا يزيد عن 150 حرف.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "رقم الهاتف مطلوب.")]
        [StringLength(20, ErrorMessage = "رقم الهاتف لا يزيد عن 20 حرف.")]
        public string Phone { get; set; } = string.Empty;

        [Required(ErrorMessage = "الجنس مطلوب.")]
        public string Gender { get; set; } = string.Empty;

        // Display-only fields (not posted back)
        public DateTime CreatedAt { get; set; }
        public int AppointmentsCount { get; set; }
        public int PrescriptionsCount { get; set; }

    }
}
