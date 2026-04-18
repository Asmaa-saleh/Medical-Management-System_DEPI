namespace Medical.PL.Data.Models
{
    public class Medicine
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string? GenericName { get; set; }
        public string? Category { get; set; }
        public string? Form { get; set; }
        public string? Strength { get; set; }

        public ICollection<PrescriptionItem> PrescriptionItems { get; set; } = new List<PrescriptionItem>();
    }
}
