using Medical.PL.Data.Models;

namespace Medical.PL.ViewModels
{
    public class SelectTimeVM
    {
        public int DoctorId { get; set; }

        public int ServiceId { get; set; }

        public DateTime SelectedDate { get; set; }

        public List<TimeSlotVM> Slots { get; set; } = new();
        public List<string> AvailableDays { get; set; } = new();
    }

    public class TimeSlotVM
    {
        public int ScheduleId { get; set; }

        public TimeSpan Time { get; set; }

        public bool IsBooked { get; set; }
    }
}
