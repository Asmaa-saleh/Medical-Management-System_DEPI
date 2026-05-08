using Medical.PL.Data.Models;
using Medical.PL.ViewModels;

namespace Medical.PL.Services
{
    public interface IPatientService
    {
        Task<IEnumerable<PatientVM>> GetAllAsync();
        Task<PatientVM?> GetByIdAsync(int id);
        Task CreateAsync(PatientVM vm);
        Task<PatientVM?> GetForEditAsync(int id);
        Task<bool> UpdateAsync(int id, PatientVM vm);
        Task<bool> DeleteAsync(int id);
        Task<PatientAppointmentsVM?> GetAppointmentsAsync(int id);
        Task<(ICollection<Prescription>?, string?, int)> GetPrescriptionsAsync(int id);

    }
}
