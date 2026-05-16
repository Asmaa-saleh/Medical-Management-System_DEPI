using System.ComponentModel.DataAnnotations;

namespace Medical.PL.ViewModels
{
    public class MedicalReportViewModel
    {
        public int AppointmentId { get; set; }
        public int DoctorId { get; set; }
        public int PatientId { get; set; }

        public string? PatientName { get; set; }
        public string? PatientPhone { get; set; }
        public string? DoctorName { get; set; }
        public string? DoctorSpecialization { get; set; }
        public string? ServiceName { get; set; }
        public DateTime AppointmentDate { get; set; }
        public TimeSpan AppointmentTime { get; set; }

        [Display(Name = "ملاحظات التقرير")]
        public string? Notes { get; set; }

        public List<MedicalReportPrescriptionItemViewModel> Items { get; set; } = new();
    }

    public class MedicalReportPrescriptionItemViewModel
    {
        [Display(Name = "الدواء")]
        public int? MedicineId { get; set; }

        [Display(Name = "الجرعة")]
        public string? Dosage { get; set; }

        [Display(Name = "الكمية")]
        public int? Quantity { get; set; }

        [Display(Name = "المدة")]
        public string? Duration { get; set; }

        [Display(Name = "التعليمات")]
        public string? Instructions { get; set; }
    }
}
