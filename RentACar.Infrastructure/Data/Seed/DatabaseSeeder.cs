using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Identity;
using RentACar.Infrastructure.Data.Models.User;

namespace RentACar.Infrastructure.Data.Seed
{
    public class DatabaseSeeder : ISeeder
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly RentCarDbContext _context;

        public DatabaseSeeder(IServiceProvider serviceProvider, RentCarDbContext context)
        {
            _serviceProvider = serviceProvider;
            _context = context;
        }

        public async Task SeedAsync()
        {
            // Order is important for data dependencies
            var seeders = new List<ISeeder>
            {
                new RoleSeeder(
                    _serviceProvider.GetRequiredService<RoleManager<IdentityRole>>(),
                    _serviceProvider.GetRequiredService<UserManager<User>>()
                ),
                new CategorySeeder(_context),
                new CarMetricsSeeder(_context),
                new RentBillSeeder(_context)
            };

            foreach (var seeder in seeders)
            {
                await seeder.SeedAsync();
            }
        }
    }
} 