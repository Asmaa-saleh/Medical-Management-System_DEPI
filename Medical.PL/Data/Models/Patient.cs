namespace Medical.PL.Data.Models
{
    public class Patient
    {
        public int Id { get; set; }
        public int UserId { get; set; }

        public User User { get; set; }
        public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
        public ICollection<Prescription> Prescriptions { get; set; } = new List<Prescription>();
    }
}
