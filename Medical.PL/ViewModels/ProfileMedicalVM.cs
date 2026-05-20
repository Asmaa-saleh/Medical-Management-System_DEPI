using Medical.PL.Data.Models;

namespace Medical.PL.ViewModels
{
    public class ProfileMedicalVM
    {
        // Basic user info
        public int UserId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public DateTime DateOfBirth { get; set; }
        public string Gender { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }

        // Medical lists
        public IEnumerable<Appointment> Appointments { get; set; } = new List<Appointment>();
        public IEnumerable<Prescription> Prescriptions { get; set; } = new List<Prescription>();
    }
}
