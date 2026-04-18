using Medical.PL.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Medical.PL.Data.Configurations
{
    public class ServiceConfiguration : IEntityTypeConfiguration<Service>
    {
       public void Configure(EntityTypeBuilder<Service> builder)
        {
            builder.HasKey(s => s.Id);

            builder.Property(s => s.Name)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(s => s.Price)
                .HasColumnType("decimal(10,2)");

            builder.HasOne(s => s.Department)
                .WithMany(d => d.Services)
                .HasForeignKey(s => s.DepartmentId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
