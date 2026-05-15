using System.ComponentModel.DataAnnotations;

namespace Medical.PL.ViewModels
{
    public class DepartmentViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "اسم القسم مطلوب")]
        [StringLength(100, ErrorMessage = "الاسم لا يجب أن يتجاوز 100 حرف")]
        [Display(Name = "اسم القسم")]
        public string Name { get; set; }

        [Display(Name = "الوصف")]
        public string? Description { get; set; }
        public bool IsDeleted { get; set; }

        [Display(Name = "عدد الأطباء")]
        public int DoctorsCount { get; set; }
    }
}