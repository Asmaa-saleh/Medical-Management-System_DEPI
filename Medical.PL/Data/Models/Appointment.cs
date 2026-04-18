namespace Medical.PL.Data.Models
{
    public class Appointment
    {
        public int Id { get; set; }
        public int PatientId { get; set; }
        public int DoctorId { get; set; }
        public int ServiceId { get; set; }
        public DateTime AppointmentDate { get; set; }
        public TimeSpan AppointmentTime { get; set; }
        public int QueueNumber { get; set; }
        public string BookingSource { get; set; }
        public string Status { get; set; }
        public string? Notes { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public Patient Patient { get; set; }
        public Doctor Doctor { get; set; }
        public Service Service { get; set; }
        public Prescription? Prescription { get; set; }
    }
}
