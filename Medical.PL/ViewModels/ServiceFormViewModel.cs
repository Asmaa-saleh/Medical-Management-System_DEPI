using System.ComponentModel.DataAnnotations;

namespace Medical.PL.ViewModels
{
    public class ServiceFormViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "اسم الخدمة مطلوب.")]
        [StringLength(100, ErrorMessage = "اسم الخدمة لا يزيد عن 100 حرف.")]
        public string Name { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "الوصف لا يزيد عن 500 حرف.")]
        public string? Description { get; set; }

        [Range(typeof(decimal), "0.01", "9999999.99", ErrorMessage = "السعر يجب أن يكون أكبر من صفر.")]
        public decimal Price { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "اختر القسم.")]
        public int DepartmentId { get; set; }
    }
}
