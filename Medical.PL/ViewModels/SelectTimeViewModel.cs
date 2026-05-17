using Medical.PL.Data.Models;

namespace Medical.PL.ViewModels
{
    public class SelectTimeViewModel
    {
        public int DoctorId { get; set; }
        public int ServiceId { get; set; }
        public List<DoctorSchedule> Schedules { get; set; }
    }
}
