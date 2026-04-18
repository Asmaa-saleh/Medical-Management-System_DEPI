namespace Medical.PL.Data.Models
{
    public class Department
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public bool IsDeleted { get; set; } = false;

        public ICollection<Doctor> Doctors { get; set; } = new List<Doctor>();
        public ICollection<Service> Services { get; set; } = new List<Service>();
    }
}
