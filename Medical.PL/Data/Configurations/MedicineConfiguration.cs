using Medical.PL.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Medical.PL.Data.Configurations
{
    public class MedicineConfiguration : IEntityTypeConfiguration<Medicine>
    {
        public void Configure(EntityTypeBuilder<Medicine> builder)
        {
            builder.HasKey(m => m.Id);

            builder.Property(m => m.Name)
                .IsRequired()
                .HasMaxLength(150);

            builder.Property(m => m.GenericName)
                .HasMaxLength(150);

            //builder.Property(m => m.Category)
            //    .HasMaxLength(100);

            //builder.Property(m => m.Form)
            //    .HasMaxLength(50);
            builder.Property(m => m.Category)
                .HasConversion<string>()
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(m => m.Form)
                .HasConversion<string>()
                .HasMaxLength(50)
                .IsRequired();


            builder.Property(m => m.Strength)
                .HasMaxLength(50);

        }
    }
}
