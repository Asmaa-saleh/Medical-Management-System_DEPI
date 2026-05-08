using Medical.PL.Data.Models;

namespace Medical.PL.ViewModels
{
    public class PatientAppointmentsVM
    {
        public int PatientId { get; set; }
        public string PatientName { get; set; } = string.Empty;
        public IEnumerable<Appointment> Appointments { get; set; } = new List<Appointment>();

    }
}
