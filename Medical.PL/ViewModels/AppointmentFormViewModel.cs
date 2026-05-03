namespace Medical.PL.ViewModels
{
    public class AppointmentFormViewModel
    {
        public int Id { get; set; }
        public string PatientMode { get; set; } = "Existing";
        public int? ExistingPatientId { get; set; }

        public string? NewPatientName { get; set; }
        public DateTime? NewPatientDateOfBirth { get; set; }
        public string? NewPatientEmail { get; set; }
        public string? NewPatientPhone { get; set; }
        public string? NewPatientGender { get; set; }

        public int ScheduleId { get; set; }
        public int ServiceId { get; set; }
        public DateTime AppointmentDate { get; set; } = DateTime.Today;
        public TimeSpan AppointmentTime { get; set; }
        public string BookingSource { get; set; } = "Clinic";
        public string Status { get; set; } = "Booked";
        public string? Notes { get; set; }
    }
}
