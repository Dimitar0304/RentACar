using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RentACar.Infrastructure.Data.Models.User;
using RentACar.Infrastructure.Data.Models.Vehicle;

namespace RentACar.Infrastructure.Data.Configurations
{
    public class ApplicationUserConfiguration : IEntityTypeConfiguration<ApplicationUser>
    {
        public void Configure(EntityTypeBuilder<ApplicationUser> builder)
        {
            builder.Property(u => u.FirstName)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(u => u.LastName)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(u => u.Email)
                .IsRequired()
                .HasMaxLength(256);

            builder.Property(u => u.Phone)
                .IsRequired()
                .HasMaxLength(20);

            builder.Property(u => u.DateRegistered)
                .IsRequired();

            builder.HasMany(u => u.RentBills)
                .WithOne(r => r.User)
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }

    public class CarConfiguration : IEntityTypeConfiguration<Car>
    {
        public void Configure(EntityTypeBuilder<Car> builder)
        {
            builder.HasKey(c => c.Id);

            builder.Property(c => c.Model)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(c => c.Make)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(c => c.ImageUrl)
                .IsRequired();

            builder.HasOne(c => c.Category)
                .WithMany(cat => cat.Cars)
                .HasForeignKey(c => c.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }

    public class CategoryConfiguration : IEntityTypeConfiguration<Category>
    {
        public void Configure(EntityTypeBuilder<Category> builder)
        {
            builder.HasKey(c => c.Id);

            builder.Property(c => c.Name)
                .IsRequired()
                .HasMaxLength(50);
        }
    }

    public class RentBillConfiguration : IEntityTypeConfiguration<RentBill>
    {
        public void Configure(EntityTypeBuilder<RentBill> builder)
        {
            builder.HasKey(rb => rb.Id);

            builder.Property(rb => rb.TownOfRent)
                .IsRequired()
                .HasMaxLength(30);

            builder.Property(rb => rb.TotalPrice)
                .HasPrecision(18, 2);

            builder.HasOne(rb => rb.User)
                .WithMany(u => u.RentBills)
                .HasForeignKey(rb => rb.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(rb => rb.Car)
                .WithMany(c => c.RentBills)
                .HasForeignKey(rb => rb.CarId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
} 