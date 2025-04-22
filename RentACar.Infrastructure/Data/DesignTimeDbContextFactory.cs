using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace RentACar.Infrastructure.Data
{
    public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<RentCarDbContext>
    {
        public RentCarDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<RentCarDbContext>();
            optionsBuilder.UseSqlServer("Server=.\\SQLEXPRESS;Database=RentACar;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True");

            return new RentCarDbContext(optionsBuilder.Options);
        }
    }
} 