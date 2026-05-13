using Medical.PL.Data.Context;
using Medical.PL.Data.Models;
using Medical.PL.Interfaces;
using Medical.PL.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace Medical.PL.Services
{
    public class PatientService : IPatientService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly AppDbContext _context;

        public PatientService(IUnitOfWork unitOfWork, AppDbContext context)
        {
            _unitOfWork = unitOfWork;
            _context = context;
        }

        public async Task<IEnumerable<PatientVM>> GetAllAsync()
        {
            var patients = await _unitOfWork.Patients.GetAllWithIncludesAsync(
                p => p.User, p => p.Appointments, p => p.Prescriptions);

            return patients
                .OrderBy(p => p.User.Name)
                .Select(MapToVM);
        }

        public async Task<PatientVM?> GetByIdAsync(int id)
        {
            var patient = await _unitOfWork.Patients.GetByIdWithIncludesAsync(
                id, p => p.User, p => p.Appointments, p => p.Prescriptions);

            return patient == null ? null : MapToVM(patient);
        }

        public async Task CreateAsync(PatientVM vm)
        {
            var user = new User
            {
                Name = vm.Name.Trim(),
                DateOfBirth = vm.DateOfBirth,
                Email = vm.Email.Trim(),
                PhoneNumber = vm.Phone.Trim(),
                Gender = vm.Gender
            };

            await _unitOfWork.Users.AddAsync(user);
            await _unitOfWork.CompleteAsync();

            var patient = new Patient { UserId = user.Id };
            await _unitOfWork.Patients.AddAsync(patient);
            await _unitOfWork.CompleteAsync();
        }

        public async Task<PatientVM?> GetForEditAsync(int id)
        {
            var patient = await _unitOfWork.Patients.GetByIdWithIncludesAsync(id, p => p.User);
            if (patient == null) return null;

            return new PatientVM
            {
                Id = patient.Id,
                UserId = patient.UserId,
                Name = patient.User.Name,
                DateOfBirth = patient.User.DateOfBirth,
                Email = patient.User.Email,
                Phone = patient.User.PhoneNumber,
                Gender = patient.User.Gender
            };
        }

        public async Task<bool> UpdateAsync(int id, PatientVM vm)
        {
            var patient = await _unitOfWork.Patients.GetByIdWithIncludesAsync(id, p => p.User);
            if (patient == null) return false;

            patient.User.Name = vm.Name.Trim();
            patient.User.DateOfBirth = vm.DateOfBirth;
            patient.User.Email = vm.Email.Trim();
            patient.User.PhoneNumber = vm.Phone.Trim();
            patient.User.Gender = vm.Gender;

            _unitOfWork.Patients.Update(patient);
            await _unitOfWork.CompleteAsync();

            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var patient = await _unitOfWork.Patients.GetByIdWithIncludesAsync(
                id, p => p.User, p => p.Appointments);

            if (patient == null) return false;
            if (patient.Appointments.Any()) return false;

            var userId = patient.UserId;

            _unitOfWork.Patients.Delete(patient);
            await _unitOfWork.CompleteAsync();

            var user = await _unitOfWork.Users.GetByIdAsync(userId);
            if (user != null)
            {
                _unitOfWork.Users.Delete(user);
                await _unitOfWork.CompleteAsync();
            }

            return true;
        }

        public async Task<PatientAppointmentsVM?> GetAppointmentsAsync(int id)
        {
            var patient = await _unitOfWork.Patients.GetByIdWithIncludesAsync(id, p => p.User);
            if (patient == null) return null;

            var appointments = await _context.Appointments
                .Include(a => a.Doctor).ThenInclude(d => d.User)
                .Include(a => a.Service)
                .Where(a => a.PatientId == id)
                .OrderByDescending(a => a.AppointmentDate)
                .ThenBy(a => a.AppointmentTime)
                .ToListAsync();

            return new PatientAppointmentsVM
            {
                PatientId = patient.Id,
                PatientName = patient.User.Name,
                Appointments = appointments
            };
        }

        public async Task<(ICollection<Prescription>?, string?, int)> GetPrescriptionsAsync(int id)
        {
            var patient = await _unitOfWork.Patients.GetByIdWithIncludesAsync(id, p => p.User);
            if (patient == null) return (null, null, 0);

            var prescriptions = await _context.Prescriptions
                .Include(p => p.Doctor).ThenInclude(d => d.User)
                .Include(p => p.Appointment)
                .Include(p => p.Items).ThenInclude(i => i.Medicine)
                .Where(p => p.PatientId == id)
                .OrderByDescending(p => p.Appointment.AppointmentDate)
                .ToListAsync();

            return (prescriptions, patient.User.Name, patient.Id);
        }

        private static PatientVM MapToVM(Patient p) => new PatientVM
        {
            Id = p.Id,
            UserId = p.UserId,
            Name = p.User.Name,
            DateOfBirth = p.User.DateOfBirth,
            Email = p.User.Email,
            Phone = p.User.PhoneNumber,
            Gender = p.User.Gender,
            CreatedAt = p.User.CreatedAt,
            AppointmentsCount = p.Appointments.Count,
            PrescriptionsCount = p.Prescriptions.Count
        };

    }
}
