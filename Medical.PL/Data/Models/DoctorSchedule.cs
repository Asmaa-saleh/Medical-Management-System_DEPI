namespace Medical.PL.Data.Models
{
    public class DoctorSchedule
    {
        public int Id { get; set; }
        public int DoctorId { get; set; }
        public string DayOfWeek { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public int MaxPatients { get; set; }
        public int MaxOnlineBooking { get; set; }
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        [Microsoft.AspNetCore.Mvc.ModelBinding.Validation.ValidateNever]
        public Doctor Doctor { get; set; } = null!;
        public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
    }
}
