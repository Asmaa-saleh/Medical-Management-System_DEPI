using Medical.PL.Data.Context;
using Medical.PL.Data.Enum;
using Medical.PL.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace Medical.PL.Data
{
    public class DbInitializer
    {
        public static async Task Seed(IApplicationBuilder applicationBuilder)
        {
            using (var serviceScope = applicationBuilder.ApplicationServices.CreateScope())
            {
                var context = serviceScope.ServiceProvider.GetService<AppDbContext>();
                var roleManager = serviceScope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole<int>>>();

                context.Database.EnsureCreated();

                // ── Roles ─────────────────────────────────────────────
                string[] roles = { "Admin", "Doctor", "Patient", "Receptionist" };

                foreach (var role in roles)
                {
                    if (!await roleManager.RoleExistsAsync(role))
                    {
                        await roleManager.CreateAsync(
                            new IdentityRole<int>(role)
                        );
                    }
                }

                // ── Admin User ───────────────────────────────────────
                var userManager = serviceScope.ServiceProvider.GetRequiredService<UserManager<User>>();

                var admins = new List<(string Name, string Email, string Password, string Gender, string Phone)>
                {
                    ("System Admin", "admin1@medicare.com", "Admin123@", "ذكر", "01000000001"),
                    ("Super Admin", "admin2@medicare.com", "Admin123@", "أنثى", "01000000002")
                };

                foreach (var item in admins)
                {
                    var existingUser = await userManager.FindByEmailAsync(item.Email);

                    if (existingUser == null)
                    {
                        var admin = new User
                        {
                            UserName = item.Email,
                            Name = item.Name,
                            Email = item.Email,
                            PhoneNumber = item.Phone,
                            Gender = item.Gender,
                            DateOfBirth = new DateTime(1990, 1, 1),
                            EmailConfirmed = true
                        };

                        var result = await userManager.CreateAsync(admin, item.Password);

                        if (result.Succeeded)
                        {
                            await userManager.AddToRoleAsync(admin, "Admin");
                        }
                    }
                }








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
                    var departments = new List<Department>
                    {
                        new Department
                        {
                            Name = "القلب",
                            Description = "تشخيص وعلاج أمراض القلب والأوعية الدموية باستخدام أحدث التقنيات الطبية"
                        },
                        new Department
                        {
                            Name = "المخ والأعصاب",
                            Description = "تشخيص وعلاج أمراض الجهاز العصبي والمخ مثل الصرع والجلطات"
                        },
                        new Department
                        {
                            Name = "العظام والمفاصل",
                            Description = "علاج الكسور، إصابات العظام، وجراحات المفاصل والعمود الفقري"
                        },
                        new Department
                        {
                            Name = "الرمد",
                            Description = "تشخيص وعلاج مشاكل الإبصار وعمليات تصحيح النظر بالليزر"
                        },
                        new Department
                        {
                            Name = "الأسنان",
                            Description = "جميع خدمات الأسنان من تنظيف، حشو، تقويم، وزراعة الأسنان"
                        },
                        new Department
                        {
                            Name = "التحاليل الطبية",
                            Description = "إجراء جميع التحاليل الطبية بدقة عالية باستخدام أحدث الأجهزة"
                        }
                    };

                    context.Departments.AddRange(departments);
                    context.SaveChanges();
                }
                // ── services ─────────────────────────────────────────────────────────
                if (!context.Services.Any())
                {
                    var cardiology = context.Departments.First(d => d.Name == "القلب");
                    var neuro = context.Departments.First(d => d.Name == "المخ والأعصاب");
                    var ortho = context.Departments.First(d => d.Name == "العظام والمفاصل");
                    var eye = context.Departments.First(d => d.Name == "الرمد");
                    var dental = context.Departments.First(d => d.Name == "الأسنان");
                    var lab = context.Departments.First(d => d.Name == "التحاليل الطبية");

                    var services = new List<Service>
                    {
                        // ================= القلب =================
                        //new Service { Name="كشف قلب", Description="فحص شامل على القلب", Price=300, DepartmentId=cardiology.Id },
                        new Service { Name="رسم قلب ECG", Description="قياس النشاط الكهربائي للقلب", Price=200, DepartmentId=cardiology.Id },
                        new Service { Name="إيكو على القلب", Description="موجات صوتية على القلب", Price=600, DepartmentId=cardiology.Id },
                        new Service { Name="اختبار جهد", Description="قياس كفاءة القلب أثناء المجهود", Price=800, DepartmentId=cardiology.Id },
                        new Service { Name="متابعة ضغط الدم", Description="متابعة مرضى الضغط المزمن", Price=150, DepartmentId=cardiology.Id },
                        new Service { Name="قسطرة تشخيصية", Description="تشخيص شرايين القلب", Price=5000, DepartmentId=cardiology.Id },

                        // ================= المخ والأعصاب =================
                        new Service { Name="كشف مخ وأعصاب", Description="تشخيص الأمراض العصبية", Price=350, DepartmentId=neuro.Id },
                        new Service { Name="رسم مخ EEG", Description="قياس نشاط المخ الكهربائي", Price=700, DepartmentId=neuro.Id },
                        new Service { Name="رنين مغناطيسي مخ", Description="تصوير دقيق للمخ", Price=1500, DepartmentId=neuro.Id },
                        new Service { Name="علاج الصرع", Description="متابعة مرضى الصرع", Price=400, DepartmentId=neuro.Id },
                        new Service { Name="علاج الجلطات", Description="إعادة تأهيل بعد الجلطات", Price=1200, DepartmentId=neuro.Id },
                        new Service { Name="اختبار أعصاب", Description="فحص الأعصاب الطرفية", Price=500, DepartmentId=neuro.Id },

                        // ================= العظام =================
                        new Service { Name="كشف عظام", Description="تشخيص مشاكل العظام", Price=250, DepartmentId=ortho.Id },
                        new Service { Name="أشعة X-Ray", Description="تصوير العظام", Price=200, DepartmentId=ortho.Id },
                        new Service { Name="جبس يد/قدم", Description="تثبيت الكسور", Price=400, DepartmentId=ortho.Id },
                        new Service { Name="علاج خشونة الركبة", Description="جلسات علاج طبيعي", Price=600, DepartmentId=ortho.Id },
                        new Service { Name="منظار ركبة", Description="تشخيص وإصلاح الركبة", Price=3000, DepartmentId=ortho.Id },
                        new Service { Name="تركيب شرائح ومسامير", Description="عمليات تثبيت الكسور", Price=7000, DepartmentId=ortho.Id },

                        // ================= العيون =================
                        new Service { Name="كشف نظر", Description="فحص الإبصار", Price=150, DepartmentId=eye.Id },
                        new Service { Name="قياس ضغط العين", Description="فحص الجلوكوما", Price=200, DepartmentId=eye.Id },
                        new Service { Name="عملية مياه بيضاء", Description="إزالة الكتاراكت", Price=4000, DepartmentId=eye.Id },
                        new Service { Name="تصحيح نظر ليزر", Description="عملية الليزك", Price=8000, DepartmentId=eye.Id },
                        new Service { Name="فحص قاع العين", Description="تشخيص الشبكية", Price=300, DepartmentId=eye.Id },

                        // ================= الأسنان =================
                        new Service { Name="كشف أسنان", Description="فحص شامل للأسنان", Price=100, DepartmentId=dental.Id },
                        new Service { Name="تنظيف جير", Description="تنظيف الأسنان", Price=300, DepartmentId=dental.Id },
                        new Service { Name="حشو أسنان", Description="علاج التسوس", Price=400, DepartmentId=dental.Id },
                        new Service { Name="خلع أسنان", Description="خلع عادي أو جراحي", Price=250, DepartmentId=dental.Id },
                        new Service { Name="تقويم أسنان", Description="تصحيح الأسنان", Price=5000, DepartmentId=dental.Id },
                        new Service { Name="زراعة أسنان", Description="تركيب أسنان صناعية", Price=10000, DepartmentId=dental.Id },
                        new Service { Name="تبييض أسنان", Description="تجميل الأسنان", Price=1200, DepartmentId=dental.Id },

                        // ================= التحاليل =================
                        new Service { Name="تحليل دم كامل", Description="CBC", Price=200, DepartmentId=lab.Id },
                        new Service { Name="تحليل سكر", Description="قياس السكر", Price=100, DepartmentId=lab.Id },
                        new Service { Name="تحليل وظائف كبد", Description="Liver Function", Price=300, DepartmentId=lab.Id },
                        new Service { Name="تحليل وظائف كلى", Description="Kidney Function", Price=300, DepartmentId=lab.Id },
                        new Service { Name="تحليل فيتامينات", Description="Vitamin D & B12", Price=500, DepartmentId=lab.Id },
                        new Service { Name="تحليل حمل", Description="Pregnancy Test", Price=150, DepartmentId=lab.Id },
                        new Service { Name="تحليل براز", Description="Stool Analysis", Price=120, DepartmentId=lab.Id },
                        new Service { Name="تحليل بول", Description="Urine Analysis", Price=120, DepartmentId=lab.Id }
                    };

                    context.Services.AddRange(services);
                    context.SaveChanges();
                }

                // ── Doctor ─────────────────────────────────────────────────────────────
                if (!context.Doctors.Any())
                {
                    var departments = context.Departments.ToList();

                    var doctorsData = new[]
                    {
                        new { Name = "Walid Amr", Email = "dr.walid.amr@medicare.com", Phone = "01012395678", Gender = "ذكر", Specialization = "Pediatrics", Experience = 15 },
                        new { Name = "Ahmed Hassan", Email = "dr.ahmed.hassan@medicare.com", Phone = "01023456789", Gender = "ذكر", Specialization = "Cardiology", Experience = 12 },
                        new { Name = "Mona Ali", Email = "dr.mona.ali@medicare.com", Phone = "01034567890", Gender = "أنثى", Specialization = "Dermatology", Experience = 8 },
                        new { Name = "Khaled Mostafa", Email = "dr.khaled.mostafa@medicare.com", Phone = "01045678901", Gender = "ذكر", Specialization = "Orthopedics", Experience = 10 },
                        new { Name = "Sara Ibrahim", Email = "dr.sara.ibrahim@medicare.com", Phone = "01056789012", Gender = "أنثى", Specialization = "Neurology", Experience = 9 },
                        new { Name = "Omar Adel", Email = "dr.omar.adel@medicare.com", Phone = "01067890123", Gender = "ذكر", Specialization = "ENT", Experience = 7 },
                        new { Name = "Nouran Magdy", Email = "dr.nouran.magdy@medicare.com", Phone = "01078901234", Gender = "أنثى", Specialization = "Gynecology", Experience = 11 },
                        new { Name = "Youssef Samy", Email = "dr.youssef.samy@medicare.com", Phone = "01089012345", Gender = "ذكر", Specialization = "Oncology", Experience = 14 },
                        new { Name = "Heba Fathy", Email = "dr.heba.fathy@medicare.com", Phone = "01112345678", Gender = "أنثى", Specialization = "Psychiatry", Experience = 6 },
                        new { Name = "Tamer Nabil", Email = "dr.tamer.nabil@medicare.com", Phone = "01090123456", Gender = "ذكر", Specialization = "Radiology", Experience = 13 },
                        new { Name = "Laila Mahmoud", Email = "dr.laila.mahmoud@medicare.com", Phone = "01145678901", Gender = "أنثى", Specialization = "Ophthalmology", Experience = 5 },
                        new { Name = "Mahmoud Gamal", Email = "dr.mahmoud.gamal@medicare.com", Phone = "01178901234", Gender = "ذكر", Specialization = "Urology", Experience = 9 },
                        new { Name = "Aya Reda", Email = "dr.aya.reda@medicare.com", Phone = "01001234567", Gender = "أنثى", Specialization = "Endocrinology", Experience = 7 },
                        new { Name = "Karim Hany", Email = "dr.karim.hany@medicare.com", Phone = "01167890123", Gender = "ذكر", Specialization = "Nephrology", Experience = 10 },
                        new { Name = "Reem Tarek", Email = "dr.reem.tarek@medicare.com", Phone = "01156789012", Gender = "أنثى", Specialization = "Gastroenterology", Experience = 8 },
                        new { Name = "Hossam Ezz", Email = "dr.hossam.ezz@medicare.com", Phone = "01190123456", Gender = "ذكر", Specialization = "Pulmonology", Experience = 12 },
                        new { Name = "Dina Sameh", Email = "dr.dina.sameh@medicare.com", Phone = "01189012345", Gender = "أنثى", Specialization = "Hematology", Experience = 6 },
                        new { Name = "Amr Salah", Email = "dr.amr.salah@medicare.com", Phone = "01123456789", Gender = "ذكر", Specialization = "General Surgery", Experience = 16 },
                        new { Name = "Farah Yasser", Email = "dr.farah.yasser@medicare.com", Phone = "01199999999", Gender = "أنثى", Specialization = "Rheumatology", Experience = 5 },
                        new { Name = "Sherif Kamal", Email = "dr.sherif.kamal@medicare.com", Phone = "01134567890", Gender = "ذكر", Specialization = "Anesthesiology", Experience = 14 }
                    };

                    var random = new Random();

                    foreach (var doc in doctorsData)
                    {
                        var existingDoc = await userManager.FindByEmailAsync(doc.Email);
                        if (existingDoc != null) continue;

                        var user = new User
                        {
                            UserName = doc.Email,
                            Name = doc.Name,
                            DateOfBirth = new DateTime(
                                random.Next(1960, 1995),
                                random.Next(1, 13),
                                random.Next(1, 28)
                            ),
                            Email = doc.Email,
                            PhoneNumber = doc.Phone,
                            Gender = doc.Gender,
                            EmailConfirmed = true
                        };

                        var result = await userManager.CreateAsync(user, "Doctor123@");
                        if (!result.Succeeded) continue;

                        await userManager.AddToRoleAsync(user, "Doctor");

                        context.Doctors.Add(new Doctor
                {
                    UserId = user.Id,
                    DepartmentId = departments[random.Next(departments.Count)].Id,
                    Specialization = doc.Specialization,
                    ExperienceYears = doc.Experience,
                    Bio = doc.Bio,
                    IsActive = true
                });
                        await context.SaveChangesAsync();
                    }
                }


                // ── Doctor ─────────────────────────────────────────────────────────────
                if (!context.Doctors.Any())
                {
                    var departments = context.Departments.ToList();

                    // إذا لم تكن هناك أقسام مضافة بعد، نأخذ قسم افتراضي أو نمنع الخطأ
                    if (departments.Any())
                    {
                        var doctorsData = new[]
                        {
            new { Name = "Walid Amr", Email = "walidamr@medicare.com", Phone = "01012395678", Gender = "ذكر", Specialization = "Pediatrics", Experience = 15 },
            new { Name = "Ahmed Hassan", Email = "dr.ahmed.hassan@medicare.com", Phone = "01023456789", Gender = "ذكر", Specialization = "Cardiology", Experience = 12 },
            new { Name = "Mona Ali", Email = "dr.mona.ali@medicare.com", Phone = "01034567890", Gender = "أنثى", Specialization = "Dermatology", Experience = 8 },
            new { Name = "Khaled Mostafa", Email = "dr.khaled.mostafa@medicare.com", Phone = "01045678901", Gender = "ذكر", Specialization = "Orthopedics", Experience = 10 },
            new { Name = "Sara Ibrahim", Email = "dr.sara.ibrahim@medicare.com", Phone = "01056789012", Gender = "أنثى", Specialization = "Neurology", Experience = 9 }
        };

                        var random = new Random();

                        foreach (var doc in doctorsData)
                        {
                            var existingDoc = await userManager.FindByEmailAsync(doc.Email);

                            if (existingDoc == null)
                            {
                                var user = new User
                                {
                                    UserName = doc.Email,
                                    Name = doc.Name,
                                    Email = doc.Email,
                                    PhoneNumber = doc.Phone,
                                    Gender = doc.Gender,
                                    DateOfBirth = new DateTime(random.Next(1975, 1995), random.Next(1, 13), random.Next(1, 28)),
                                    EmailConfirmed = false
                                };

                                // إنشاء حساب المستخدم بكلمة المرور الافتراضية للدكاترة
                                var result = await userManager.CreateAsync(user, "Doctor123@");

                                if (result.Succeeded)
                                {
                                    // إسناد صلاحية دكتور للمستخدم
                                    await userManager.AddToRoleAsync(user, "Doctor");

                                    // إضافة الطبيب إلى جدول الـ Doctors وربطه بحساب الـ User الجديد
                                    var doctorEntity = new Doctor
                                    {
                                        UserId = user.Id, // الـ Id هنا بيسمع تلقائياً بعد نجاح CreateAsync
                                        DepartmentId = departments[random.Next(departments.Count)].Id, // اختيار قسم عشوائي من اللي عندك
                                        Specialization = doc.Specialization,
                                        ExperienceYears = doc.Experience,
                                        IsActive = true
                                    };

                                    context.Doctors.Add(doctorEntity);
                                    // حفظ التغييرات فوراً لكل دكتور لضمان عدم حدوث تعارض في الـ Scopes
                                    await context.SaveChangesAsync();
                                }
                            }
                        }
                    }
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
                    //var userSara = new User
                    //{
                    //    Name = "Sara Mohamed",
                    //    DateOfBirth = new DateTime(2019, 5, 20),
                    //    Email = "sara.mohamed@email.com",
                    //    PhoneNumber = "01098765432",
                    //    Gender = "أنثى"
                    //};
                    //var userKarim = new User
                    //{
                    //    Name = "Karim Ibrahim",
                    //    DateOfBirth = new DateTime(2023, 11, 30),
                    //    Email = "karim.ibrahim@email.com",
                    //    PhoneNumber = "01112223344",
                    //    Gender = "ذكر"
                    //};
                    var userSara = new User
                    {
                        UserName = "sara.mohamed@email.com",
                        Name = "Sara Mohamed",
                        DateOfBirth = new DateTime(2019, 5, 20),
                        Email = "sara.mohamed@email.com",
                        PhoneNumber = "01098765432",
                        Gender = "أنثى",
                        EmailConfirmed = true
                    };

                    var userKarim = new User
                    {
                        UserName = "karim.ibrahim@email.com",
                        Name = "Karim Ibrahim",
                        DateOfBirth = new DateTime(2023, 11, 30),
                        Email = "karim.ibrahim@email.com",
                        PhoneNumber = "01112223344",
                        Gender = "ذكر",
                        EmailConfirmed = true
                    };

                    context.Users.AddRange(userSara, userKarim);
                    context.SaveChanges();

                    context.Patients.AddRange(
                        new Patient { UserId = userSara.Id },
                        new Patient { UserId = userKarim.Id }
                    );
                    context.SaveChanges();
                }

                // ── Appointments ───────────────────────────────────────────────────────
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
                            Status = "Completed",
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
                            Status = "Booked",
                            Notes = "Follow-up for Acute Viral Gastroenteritis"
                        }
                    );
                    context.SaveChanges();
                }

                // ── Prescriptions (Pediatric — Acute Viral Gastroenteritis) ───────────
                if (!context.Prescriptions.Any())
                {
                    var doctor = context.Doctors.First();
                    var patient1 = context.Patients.First();

                    var appointment = context.Appointments
                        .Where(a => a.PatientId == patient1.Id && a.Status == "Completed")
                        .OrderBy(a => a.AppointmentDate)
                        .FirstOrDefault();

                    var efferalgan = context.Medicines.First(m => m.Name == "Efferalgan");
                    var flagyl = context.Medicines.First(m => m.Name == "Flagyl");
                    var eucarbon = context.Medicines.First(m => m.Name == "Eucarbon");
                    var imodium = context.Medicines.First(m => m.Name == "Imodium");

                    var prescription = new Prescription
                    {
                        AppointmentId = appointment.Id,
                        DoctorId = doctor.Id,
                        PatientId = patient1.Id,
                        Notes = "يُنصح بالراحة التامة والإكثار من شرب السوائل ومحاليل الإماهة الفموية (ORS). تجنب الأطعمة الدهنية والحارة ومنتجات الألبان حتى التعافي. مراجعة الطبيب فوراً في حالة ارتفاع درجة الحرارة فوق 39 أو استمرار الأعراض أكثر من 48 ساعة.",
                        Items = new List<PrescriptionItem>
                        {
                            new PrescriptionItem
                            {
                                MedicineId   = efferalgan.Id,
                                Dosage       = "125 mg",
                                Quantity     = 1,
                                Duration     = "5 أيام",
                                Instructions = "ملعقة صغيرة (5 مل) كل 6 ساعات عند الحاجة لخفض الحرارة أو تخفيف الألم"
                            },
                            new PrescriptionItem
                            {
                                MedicineId   = flagyl.Id,
                                Dosage       = "125 mg",
                                Quantity     = 15,
                                Duration     = "5 أيام",
                                Instructions = "نصف قرص ثلاث مرات يومياً بعد الأكل — جرعة مخصصة للأطفال"
                            },
                            new PrescriptionItem
                            {
                                MedicineId   = eucarbon.Id,
                                Dosage       = "250 mg",
                                Quantity     = 10,
                                Duration     = "3 أيام",
                                Instructions = "قرص واحد مرتين يومياً بعد الوجبات لامتصاص السموم وتخفيف الانتفاخ"
                            },
                            new PrescriptionItem
                            {
                                MedicineId   = imodium.Id,
                                Dosage       = "1 mg",
                                Quantity     = 6,
                                Duration     = "عند الحاجة",
                                Instructions = "نصف كبسولة بعد كل إسهال، بحد أقصى 3 جرعات يومياً — للأطفال فوق 6 سنوات"
                            }
                        }
                    };

                    context.Prescriptions.Add(prescription);
                    context.SaveChanges();
                }
            }
        }
    }
}
