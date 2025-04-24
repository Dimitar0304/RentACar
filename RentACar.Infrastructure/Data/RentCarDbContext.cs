using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using RentACar.Infrastructure.Data.Configurations;
using RentACar.Infrastructure.Data.Models.User;
using RentACar.Infrastructure.Data.Models.Vehicle;
using System.Reflection.Emit;

namespace RentACar.Infrastructure.Data
{
    public class RentCarDbContext : IdentityDbContext<ApplicationUser>
    {
        public RentCarDbContext(DbContextOptions<RentCarDbContext> options)
            : base(options)
        {
        }

        public DbSet<Car> Cars { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<RentBill> Bills { get; set; }
        public DbSet<CarMetrics> CarMetrics { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Apply entity configurations
            builder.ApplyConfiguration(new ApplicationUserConfiguration());
            builder.ApplyConfiguration(new CarConfiguration());
            builder.ApplyConfiguration(new CarMetricsConfiguration());
            builder.ApplyConfiguration(new CategoryConfiguration());
            builder.ApplyConfiguration(new RentBillConfiguration());

            builder.Entity<Car>()
                .HasOne(c => c.Category)
                .WithMany(c => c.Cars)
                .HasForeignKey(c => c.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

           builder.Entity<Car>()
                .HasMany(c => c.RentBills)
                .WithOne(rb => rb.Car)
                .HasForeignKey(rb => rb.CarId)
                .OnDelete(DeleteBehavior.Restrict);

           builder.Entity<Car>()
                .HasOne(c => c.Metrics)
                .WithOne(m => m.Car)
                .HasForeignKey<CarMetrics>(m => m.CarId);
        }
    }
}
