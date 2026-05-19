using System.ComponentModel.DataAnnotations;

namespace Medical.PL.ViewModels
{
    public class EditPrescriptionViewModel
    {
        public int PrescriptionId { get; set; }
        public int DoctorId { get; set; }
        public string? DoctorName { get; set; }
        public string? PatientName { get; set; }
        public DateTime? AppointmentDate { get; set; }
        public TimeSpan? AppointmentTime { get; set; }
        public string? ServiceName { get; set; }

        [Display(Name = "ملاحظات التقرير")]
        public string? Notes { get; set; }

        public List<MedicalReportPrescriptionItemViewModel> Items { get; set; } = new();
    }
}
