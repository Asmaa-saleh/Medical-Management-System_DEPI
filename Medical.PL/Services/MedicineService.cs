using Medical.PL.Data.Context;
using Medical.PL.Data.Models;
using Medical.PL.Repositories;
using Medical.PL.Services;
using Microsoft.EntityFrameworkCore;


namespace Medical.PL.Services
{
    public class MedicineService : GenericRepository<Medicine>,IMedicineService
    {
        //protected readonly AppDbContext _context;
        public MedicineService(AppDbContext context) : base(context)
        { 
            
        }
        public async Task<IEnumerable<Medicine>> SearchAsync(string term)
        {
            if (string.IsNullOrWhiteSpace(term))
                return await GetAllAsync();

            return await FindAsync(m =>
                m.Name.ToLower().Contains(term.ToLower()) ||
                (m.GenericName != null && m.GenericName.ToLower().Contains(term.ToLower()))
            );
        }


    }
}


