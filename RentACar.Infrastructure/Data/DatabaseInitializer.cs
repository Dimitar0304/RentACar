using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RentACar.Infrastructure.Data.Seed;

namespace RentACar.Infrastructure.Data
{
    public static class DatabaseInitializer
    {
        public static async Task InitializeAsync(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var services = scope.ServiceProvider;

            try
            {
                var context = services.GetRequiredService<RentCarDbContext>();
                var logger = services.GetRequiredService<ILogger<RentCarDbContext>>();

                // Apply migrations
                await context.Database.MigrateAsync();
                logger.LogInformation("Database migrations applied successfully.");

                // Run seeders
                var seeders = new List<ISeeder>
                {
                    new CarMetricsSeeder(context)
                    // Add other seeders here as needed
                };

                foreach (var seeder in seeders)
                {
                    await seeder.SeedAsync();
                    logger.LogInformation($"Seeder {seeder.GetType().Name} completed successfully.");
                }

                await context.SaveChangesAsync();
                logger.LogInformation("Database initialization completed successfully.");
            }
            catch (Exception ex)
            {
                var logger = services.GetRequiredService<ILogger<RentCarDbContext>>();
                logger.LogError(ex, "An error occurred while initializing the database.");
                throw;
            }
        }
    }
} 