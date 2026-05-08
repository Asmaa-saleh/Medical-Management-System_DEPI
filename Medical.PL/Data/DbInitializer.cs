using Medical.PL.Data.Context;
using Medical.PL.Data.Enum;
using Medical.PL.Data.Models;

namespace Medical.PL.Data
{
    public class DbInitializer
    {
        public static void Seed(IApplicationBuilder applicationBuilder)
        {
            using (var serviceScope = applicationBuilder.ApplicationServices.CreateScope())
            {
                var context = serviceScope.ServiceProvider.GetService<AppDbContext>();
                
                context.Database.EnsureCreated();

                if (!context.Medicines.Any())
                {
                    var medicines = new List<Medicine>
{
    new Medicine { Name="Panadol", GenericName="Paracetamol", Category=MedicineCategory.Analgesic, Form=MedicineForm.Tablet, Strength="500 mg" },
    new Medicine { Name="Panadol Extra", GenericName="Paracetamol + Caffeine", Category=MedicineCategory.Analgesic, Form=MedicineForm.Tablet, Strength="500/65 mg" },
    new Medicine { Name="Brufen", GenericName="Ibuprofen", Category=MedicineCategory.AntiInflammatory, Form=MedicineForm.Tablet, Strength="400 mg" },
    new Medicine { Name="Cataflam", GenericName="Diclofenac Potassium", Category=MedicineCategory.AntiInflammatory, Form=MedicineForm.Tablet, Strength="50 mg" },
    new Medicine { Name="Voltaren", GenericName="Diclofenac Sodium", Category=MedicineCategory.AntiInflammatory, Form=MedicineForm.Gel, Strength="1%" },

    new Medicine { Name="Augmentin", GenericName="Amoxicillin + Clavulanic Acid", Category=MedicineCategory.Antibiotic, Form=MedicineForm.Tablet, Strength="625 mg" },
    new Medicine { Name="Amoxil", GenericName="Amoxicillin", Category=MedicineCategory.Antibiotic, Form=MedicineForm.Capsule, Strength="500 mg" },
    new Medicine { Name="Zithromax", GenericName="Azithromycin", Category=MedicineCategory.Antibiotic, Form=MedicineForm.Tablet, Strength="500 mg" },
    new Medicine { Name="Flagyl", GenericName="Metronidazole", Category=MedicineCategory.Antibiotic, Form=MedicineForm.Tablet, Strength="500 mg" },
    new Medicine { Name="Cipro", GenericName="Ciprofloxacin", Category=MedicineCategory.Antibiotic, Form=MedicineForm.Tablet, Strength="500 mg" },

    new Medicine { Name="Clarinase", GenericName="Loratadine + Pseudoephedrine", Category=MedicineCategory.Antihistamine, Form=MedicineForm.Tablet, Strength="5/120 mg" },
    new Medicine { Name="Zyrtec", GenericName="Cetirizine", Category=MedicineCategory.Antihistamine, Form=MedicineForm.Tablet, Strength="10 mg" },
    new Medicine { Name="Telfast", GenericName="Fexofenadine", Category=MedicineCategory.Antihistamine, Form=MedicineForm.Tablet, Strength="120 mg" },

    new Medicine { Name="Otrivin", GenericName="Xylometazoline", Category=MedicineCategory.Decongestant, Form=MedicineForm.Spray, Strength="0.1%" },
    new Medicine { Name="Nasacort", GenericName="Triamcinolone", Category=MedicineCategory.Decongestant, Form=MedicineForm.Spray, Strength="55 mcg" },

    new Medicine { Name="Vitamin C", GenericName="Ascorbic Acid", Category=MedicineCategory.Vitamin, Form=MedicineForm.Tablet, Strength="1000 mg" },
    new Medicine { Name="Vitamin D3", GenericName="Cholecalciferol", Category=MedicineCategory.Vitamin, Form=MedicineForm.Capsule, Strength="1000 IU" },
    new Medicine { Name="Centrum", GenericName="Multivitamins", Category=MedicineCategory.Vitamin, Form=MedicineForm.Tablet, Strength="Multivitamin" },

    new Medicine { Name="Insulin", GenericName="Human Insulin", Category=MedicineCategory.Hormone, Form=MedicineForm.Injection, Strength="100 IU/ml" },
    new Medicine { Name="Glucophage", GenericName="Metformin", Category=MedicineCategory.Antidiabetic, Form=MedicineForm.Tablet, Strength="500 mg" },

    new Medicine { Name="Aspirin", GenericName="Acetylsalicylic Acid", Category=MedicineCategory.Analgesic, Form=MedicineForm.Tablet, Strength="75 mg" },
    new Medicine { Name="Ketolac", GenericName="Ketorolac", Category=MedicineCategory.Analgesic, Form=MedicineForm.Tablet, Strength="10 mg" },

    new Medicine { Name="Eucarbon", GenericName="Activated Charcoal", Category=MedicineCategory.Gastrointestinal, Form=MedicineForm.Tablet, Strength="250 mg" },
    new Medicine { Name="Imodium", GenericName="Loperamide", Category=MedicineCategory.Gastrointestinal, Form=MedicineForm.Capsule, Strength="2 mg" },

    new Medicine { Name="Ventolin", GenericName="Salbutamol", Category=MedicineCategory.Bronchodilator, Form=MedicineForm.Inhaler, Strength="100 mcg" },
    new Medicine { Name="Symbicort", GenericName="Budesonide + Formoterol", Category=MedicineCategory.Bronchodilator, Form=MedicineForm.Inhaler, Strength="160/4.5 mcg" },

    new Medicine { Name="Losec", GenericName="Omeprazole", Category=MedicineCategory.Gastrointestinal, Form=MedicineForm.Capsule, Strength="20 mg" },
    new Medicine { Name="Nexium", GenericName="Esomeprazole", Category=MedicineCategory.Gastrointestinal, Form=MedicineForm.Tablet, Strength="40 mg" },

    new Medicine { Name="Heparin", GenericName="Heparin Sodium", Category=MedicineCategory.Anticoagulant, Form=MedicineForm.Injection, Strength="5000 IU" },
    new Medicine { Name="Warfarin", GenericName="Warfarin", Category=MedicineCategory.Anticoagulant, Form=MedicineForm.Tablet, Strength="5 mg" },

    new Medicine { Name="Lasix", GenericName="Furosemide", Category=MedicineCategory.Diuretic, Form=MedicineForm.Tablet, Strength="40 mg" },
    new Medicine { Name="Aldactone", GenericName="Spironolactone", Category=MedicineCategory.Diuretic, Form=MedicineForm.Tablet, Strength="25 mg" },

    new Medicine { Name="Betadine", GenericName="Povidone Iodine", Category=MedicineCategory.Antiseptic, Form=MedicineForm.Solution, Strength="10%" },
    new Medicine { Name="Dettol", GenericName="Chloroxylenol", Category=MedicineCategory.Antiseptic, Form=MedicineForm.Solution, Strength="4.8%" },

    new Medicine { Name="Tramal", GenericName="Tramadol", Category=MedicineCategory.Analgesic, Form=MedicineForm.Capsule, Strength="50 mg" },
    new Medicine { Name="Morphine", GenericName="Morphine Sulfate", Category=MedicineCategory.Analgesic, Form=MedicineForm.Injection, Strength="10 mg/ml" },

    new Medicine { Name="Lantus", GenericName="Insulin Glargine", Category=MedicineCategory.Hormone, Form=MedicineForm.Injection, Strength="100 IU/ml" },
    new Medicine { Name="Januvia", GenericName="Sitagliptin", Category=MedicineCategory.Antidiabetic, Form=MedicineForm.Tablet, Strength="100 mg" },

    new Medicine { Name="Duspatalin", GenericName="Mebeverine", Category=MedicineCategory.Gastrointestinal, Form=MedicineForm.Tablet, Strength="135 mg" },
    new Medicine { Name="Buscopan", GenericName="Hyoscine", Category=MedicineCategory.Gastrointestinal, Form=MedicineForm.Tablet, Strength="10 mg" },

    new Medicine { Name="Neurobion", GenericName="Vitamin B Complex", Category=MedicineCategory.Vitamin, Form=MedicineForm.Tablet, Strength="B1+B6+B12" },
    new Medicine { Name="Feroglobin", GenericName="Iron Supplement", Category=MedicineCategory.Supplement, Form=MedicineForm.Syrup, Strength="Iron" },

    new Medicine { Name="Calcium D", GenericName="Calcium + Vitamin D", Category=MedicineCategory.Supplement, Form=MedicineForm.Tablet, Strength="600 mg" },
    new Medicine { Name="Omega 3", GenericName="Fish Oil", Category=MedicineCategory.Supplement, Form=MedicineForm.Capsule, Strength="1000 mg" },

    new Medicine { Name="Efferalgan", GenericName="Paracetamol", Category=MedicineCategory.Analgesic, Form=MedicineForm.Syrup, Strength="250 mg" },
    new Medicine { Name="Doliprane", GenericName="Paracetamol", Category=MedicineCategory.Analgesic, Form=MedicineForm.Tablet, Strength="500 mg" }
};
              

                    context.Medicines.AddRange(medicines);
                    context.SaveChanges();
                }


                // ── Department ─────────────────────────────────────────────────────────
                if (!context.Departments.Any())
                {
                    context.Departments.Add(new Department
                    {
                        Name = "Pediatrics",
                        Description = "Focuses on well-baby checkups, developmental assessments, and routine screenings."
                    });
                    context.SaveChanges();
                }

                // ── Doctor ─────────────────────────────────────────────────────────────
                if (!context.Doctors.Any())
                {
                    var dept = context.Departments.First();

                    var doctorUser = new User
                    {
                        Name = "Walid Amr",
                        DateOfBirth = new DateTime(1960, 3, 15),
                        Email = "dr.walid.amr@medicare.com",
                        Phone = "01012395678",
                        Gender = "ذكر"
                    };
                    context.Users.Add(doctorUser);
                    context.SaveChanges();

                    context.Doctors.Add(new Doctor
                    {
                        UserId = doctorUser.Id,
                        DepartmentId = dept.Id,
                        Specialization = "Pediatrics",
                        ExperienceYears = 15,
                        IsActive = true
                    });
                    context.SaveChanges();
                }

                // ── Service ────────────────────────────────────────────────────────────
                if (!context.Services.Any())
                {
                    var dept = context.Departments.First();

                    context.Services.Add(new Service
                    {
                        Name = "General Consultation",
                        Description = "Standard outpatient consultation",
                        Price = 200m,
                        DepartmentId = dept.Id
                    });
                    context.SaveChanges();
                }

                // ── Doctor Schedule ────────────────────────────────────────────────────
                if (!context.DoctorSchedules.Any())
                {
                    var doctor = context.Doctors.First();

                    context.DoctorSchedules.Add(new DoctorSchedule
                    {
                        DoctorId = doctor.Id,
                        DayOfWeek = "الأحد",
                        StartTime = new TimeSpan(9, 0, 0),
                        EndTime = new TimeSpan(17, 0, 0),
                        MaxPatients = 20,
                        MaxOnlineBooking = 10
                    });
                    context.SaveChanges();
                }

                // ── Patients ───────────────────────────────────────────────────────────
                if (!context.Patients.Any())
                {
                    var userSara = new User
                    {
                        Name = "Sara Mohamed",
                        DateOfBirth = new DateTime(2019, 5, 20),
                        Email = "sara.mohamed@email.com",
                        Phone = "01098765432",
                        Gender = "أنثى"
                    };
                    var userKarim = new User
                    {
                        Name = "Karim Ibrahim",
                        DateOfBirth = new DateTime(2023, 11, 30),
                        Email = "karim.ibrahim@email.com",
                        Phone = "01112223344",
                        Gender = "ذكر"
                    };

                    context.Users.AddRange(userSara, userKarim);
                    context.SaveChanges();

                    context.Patients.AddRange(
                        new Patient { UserId = userSara.Id },
                        new Patient { UserId = userKarim.Id }
                    );
                    context.SaveChanges();
                }

                // ── Appointments ─────────────────────────────
                if (!context.Appointments.Any())
                {
                    var doctor = context.Doctors.First();
                    var service = context.Services.First();
                    var schedule = context.DoctorSchedules.First();
                    var patient1 = context.Patients.First();

                    context.Appointments.AddRange(
                        new Appointment
                        {
                            PatientId = patient1.Id,
                            DoctorId = doctor.Id,
                            ServiceId = service.Id,
                            ScheduleId = schedule.Id,
                            AppointmentDate = new DateTime(2026, 5, 3),
                            AppointmentTime = new TimeSpan(9, 0, 0),
                            QueueNumber = 1,
                            BookingSource = "Clinic",
                            Status = "تم الكشف",
                            Notes = "Diagnosed with Acute Viral Gastroenteritis"
                        },
                        new Appointment
                        {
                            PatientId = patient1.Id,
                            DoctorId = doctor.Id,
                            ServiceId = service.Id,
                            ScheduleId = schedule.Id,
                            AppointmentDate = new DateTime(2026, 5, 10),
                            AppointmentTime = new TimeSpan(11, 0, 0),
                            QueueNumber = 2,
                            BookingSource = "Clinic",
                            Status = "محجوز",
                            Notes = "Follow-up for Acute Viral Gastroenteritis"
                        }
                    );
                    context.SaveChanges();
                }

            }
        }
    }
}
