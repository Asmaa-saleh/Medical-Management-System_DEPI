namespace Medical.PL.Data.Models
{
    public class Service
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public int DepartmentId { get; set; }
        public bool IsDeleted { get; set; } = false;

        public Department Department { get; set; }
        public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
    }
}
