using Medical.PL.Data.Enum;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace Medical.PL.ViewModels
{
    public class MedicineVM
    {
        [Required]
        public string Name { get; set; }

        public string? GenericName { get; set; }

        [Required]
        public MedicineCategory Category { get; set; }

        [Required]
        public MedicineForm Form { get; set; }

        public string? Strength { get; set; }

        
    }
}
