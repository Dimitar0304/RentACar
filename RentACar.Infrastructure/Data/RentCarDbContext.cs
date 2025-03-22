using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using RentACar.Infrastructure.Data.Models.User;

namespace RentACar.Infrastructure.Data
{
     public class RentCarDbContext : IdentityDbContext<User>
    {

        public RentCarDbContext(DbContextOptions<RentCarDbContext> options) : base(options)
        {
        }
        public DbSet<User> Users { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
        }
    }
}
