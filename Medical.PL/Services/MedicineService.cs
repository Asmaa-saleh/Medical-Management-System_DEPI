using Medical.PL.Data.Context;
using Medical.PL.Data.Models;
using Medical.PL.Repositories;


namespace Medical.PL.Services
{
    public class MedicineService : GenericRepository<Medicine>,IMedicineService
    {
        //protected readonly AppDbContext _context;
        public MedicineService(AppDbContext context) : base(context)
        {  }
    }
}
