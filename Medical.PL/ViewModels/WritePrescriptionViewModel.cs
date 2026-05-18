using System.ComponentModel.DataAnnotations;

namespace Medical.PL.ViewModels
{
    public class WritePrescriptionViewModel
    {
        public int DoctorId { get; set; }
        public string? DoctorName { get; set; }

        [Required(ErrorMessage = "يجب اختيار الحجز")]
        [Display(Name = "الحجز")]
        public int AppointmentId { get; set; }

        [Display(Name = "ملاحظات التقرير")]
        public string? Notes { get; set; }

        public List<MedicalReportPrescriptionItemViewModel> Items { get; set; } = new();
    }
}
