using Microsoft.EntityFrameworkCore;

namespace RentACar.Infrastructure.Data.Seed
{
    public class ApplicationSeeder
    {
        private readonly IEnumerable<ISeeder> seeders;

        public ApplicationSeeder(IEnumerable<ISeeder> _seeders)
        {
            seeders = _seeders;
        }
        public async Task SeedAsync()
        {
            
                foreach (var seeder in seeders)
                {
                    await seeder.SeedAsync();
                }
            
        }
    }
}
