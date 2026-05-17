namespace Medical.PL.ViewModels
{
    public class ConfirmBookingVM
    {
        public int ServiceId { get; set; }
        public int DoctorId { get; set; }
        public int ScheduleId { get; set; }

        public DateTime Date { get; set; }
        public TimeSpan Time { get; set; }

        public PatientVM Patient { get; set; } = new();

        // Display Data
        public string ServiceName { get; set; } = string.Empty;
        public string DoctorName { get; set; } = string.Empty;
        public string DayName { get; set; } = string.Empty;
    }
}
