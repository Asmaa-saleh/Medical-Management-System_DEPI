using Medical.PL.Data.Context;
using Medical.PL.Data.Models;
using Medical.PL.Interfaces;

namespace Medical.PL.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;

        public IGenericRepository<User> Users { get; private set; }
        public IGenericRepository<Patient> Patients { get; private set; }
        public IGenericRepository<Doctor> Doctors { get; private set; }
        public IGenericRepository<Department> Departments { get; private set; }
        public IGenericRepository<Service> Services { get; private set; }
        public IGenericRepository<Medicine> Medicines { get; private set; }
        public IGenericRepository<Appointment> Appointments { get; private set; }
        public IGenericRepository<Prescription> Prescriptions { get; private set; }
        public IGenericRepository<PrescriptionItem> PrescriptionItems { get; private set; }
        public IGenericRepository<DoctorSchedule> DoctorSchedules { get; private set; }
        public UnitOfWork(AppDbContext context)
        {
            _context = context;

            Users = new GenericRepository<User>(context);
            Patients = new GenericRepository<Patient>(context);
            Doctors = new GenericRepository<Doctor>(context);
            Departments = new GenericRepository<Department>(context);
            Services = new GenericRepository<Service>(context);
            Medicines = new GenericRepository<Medicine>(context);
            Appointments = new GenericRepository<Appointment>(context);
            Prescriptions = new GenericRepository<Prescription>(context);
            PrescriptionItems = new GenericRepository<PrescriptionItem>(context);
            DoctorSchedules = new GenericRepository<DoctorSchedule>(context);
        }

        public async Task<int> CompleteAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
