using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Identity;
using RentACar.Infrastructure.Data.Models.User;
using RentACar.Infrastructure.Data.Models.Vehicle;

namespace RentACar.Infrastructure.Data.Seed
{
    public class DatabaseSeeder
    {
        private readonly IServiceProvider _serviceProvider;

        public DatabaseSeeder(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task SeedAsync()
        {
            var seeders = new List<ISeeder>
            {
                new RoleSeeder(
                    _serviceProvider.GetRequiredService<RoleManager<IdentityRole>>(),
                    _serviceProvider.GetRequiredService<UserManager<ApplicationUser>>()
                ),
                new CategorySeeder(_serviceProvider.GetRequiredService<RentCarDbContext>()),
                new CarSeeder(_serviceProvider.GetRequiredService<RentCarDbContext>()),
                new CarMetricsSeeder(_serviceProvider.GetRequiredService<RentCarDbContext>()),
                new RentBillSeeder(_serviceProvider.GetRequiredService<RentCarDbContext>())
            };

            foreach (var seeder in seeders)
            {
                await seeder.SeedAsync();
            }
        }
    }
} 