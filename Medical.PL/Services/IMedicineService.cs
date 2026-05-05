using Medical.PL.Data.Models;
using Medical.PL.Interfaces;

namespace Medical.PL.Services
{
    public interface IMedicineService :IGenericRepository<Medicine>
    {
        Task<IEnumerable<Medicine>> SearchAsync(string term);
    }
}
