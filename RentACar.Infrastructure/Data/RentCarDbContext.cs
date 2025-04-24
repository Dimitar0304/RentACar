using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using RentACar.Infrastructure.Data.Models.User;
using RentACar.Infrastructure.Data.Models.Vehicle;
using System.Reflection.Emit;

namespace RentACar.Infrastructure.Data
{
    public class RentCarDbContext : IdentityDbContext<User>
    {

        public RentCarDbContext(DbContextOptions<RentCarDbContext> options) : base(options)
        {
        }
        public DbSet<User> Users { get; set; } = null!;

        public DbSet<RentBill> Bills { get; set; } = null!;

        public DbSet<Category> Categories { get; set; } = null!;

        public DbSet<Car> Cars { get; set; } = null!;

        public DbSet<CarMetrics> CarMetrics { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Car>()
                .HasOne(c => c.Category)
                .WithMany(c => c.Cars)
                .HasForeignKey(c => c.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

           builder.Entity<Car>()
                .HasOne(c => c.RentBill)
                .WithOne(rb => rb.Car)
                .HasForeignKey<RentBill>(rb => rb.CarId);

           builder.Entity<Car>()
                .HasOne(c => c.Metrics)
                .WithOne(m => m.Car)
                .HasForeignKey<CarMetrics>(m => m.CarId);

        }
    }
}
