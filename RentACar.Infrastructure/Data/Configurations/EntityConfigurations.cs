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

            builder.HasMany(c => c.RentBills)
                .WithOne(r => r.Car)
                .HasForeignKey(r => r.CarId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(c => c.Metrics)
                .WithOne(m => m.Car)
                .HasForeignKey<CarMetrics>(m => m.CarId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }

    public class CarMetricsConfiguration : IEntityTypeConfiguration<CarMetrics>
    {
        public void Configure(EntityTypeBuilder<CarMetrics> builder)
        {
            builder.HasKey(m => m.Id);

            builder.Property(m => m.EngineTemperature)
                .IsRequired();

            builder.Property(m => m.OilLevel)
                .IsRequired();

            builder.Property(m => m.TireWear)
                .IsRequired();

            builder.Property(m => m.BrakeWear)
                .IsRequired();

            builder.Property(m => m.LastServiceDate)
                .IsRequired();

            builder.Property(m => m.LastUpdated)
                .IsRequired();
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

            builder.HasMany(c => c.Cars)
                .WithOne(car => car.Category)
                .HasForeignKey(car => car.CategoryId);
        }
    }

    public class RentBillConfiguration : IEntityTypeConfiguration<RentBill>
    {
        public void Configure(EntityTypeBuilder<RentBill> builder)
        {
            builder.HasKey(r => r.Id);

            builder.Property(r => r.DateOfTaking)
                .IsRequired();

            builder.Property(r => r.DateOfReturn)
                .IsRequired(false);

            builder.Property(r => r.StartMileage)
                .IsRequired();

            builder.Property(r => r.EndMileage)
                .IsRequired(false);

            builder.Property(r => r.TotalPrice)
                .IsRequired()
                .HasPrecision(18, 2);

            builder.HasOne(r => r.Car)
                .WithMany(c => c.RentBills)
                .HasForeignKey(r => r.CarId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(r => r.User)
                .WithMany(u => u.RentBills)
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
} 