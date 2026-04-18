namespace Medical.PL.Data.Models
{
    public class Doctor
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int DepartmentId { get; set; }
        public string Specialization { get; set; }
        public int ExperienceYears { get; set; }
        public string? Bio { get; set; }
        public string? Image { get; set; }
        public bool IsActive { get; set; } = true;

        public User User { get; set; }
        public Department Department { get; set; }
        public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
        public ICollection<Prescription> Prescriptions { get; set; } = new List<Prescription>();
    }
}
