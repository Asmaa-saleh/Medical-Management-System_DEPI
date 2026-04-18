using Medical.PL.Data.Models;

namespace Medical.PL.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IGenericRepository<User> Users { get; }
        IGenericRepository<Patient> Patients { get; }
        IGenericRepository<Doctor> Doctors { get; }
        IGenericRepository<Department> Departments { get; }
        IGenericRepository<Service> Services { get; }
        IGenericRepository<Medicine> Medicines { get; }
        IGenericRepository<Appointment> Appointments { get; }
        IGenericRepository<Prescription> Prescriptions { get; }
        IGenericRepository<PrescriptionItem> PrescriptionItems { get; }

        Task<int> CompleteAsync();
    }
}
