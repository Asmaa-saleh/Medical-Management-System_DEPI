using Medical.PL.Data.Enum;
using System.ComponentModel.DataAnnotations;

namespace Medical.PL.Data.Models
{
    public class Medicine
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        public string? GenericName { get; set; }
        public MedicineCategory? Category { get; set; }
        public MedicineForm? Form { get; set; }
        public string? Strength { get; set; }

        public ICollection<PrescriptionItem> PrescriptionItems { get; set; } = new List<PrescriptionItem>();
    }
}
