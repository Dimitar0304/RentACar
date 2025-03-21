using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using RentACar.Infrastructure.Data.Models;
using RentACar.Infrastructure.Data.Models.User;

namespace RentACar.Infrastructure.Data
{
     public class ApplicationDbContext : IdentityDbContext<User>
    {

        public ApplicationDbContext(DbContextOptions options) : base(options)
        {
        }
        public DbSet<User> Users { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
        }
    }
}
