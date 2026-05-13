using System.ComponentModel.DataAnnotations;

namespace Medical.PL.ViewModels
{
    public class RoleViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "اسم الدور مطلوب")]
        [StringLength(100, ErrorMessage = "اسم الدور لا يجب أن يتجاوز 100 حرف")]
        [Display(Name = "اسم الدور")]
        public string Name { get; set; } = string.Empty;

        public int UsersCount { get; set; }

        public List<string> Users { get; set; } = [];
    }
}
